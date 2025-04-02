using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using App.Config;
using App.DataCache;
using Cysharp.Threading.Tasks;
using DFDev.EventSystem;
using DFDev.Singleton;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace App.UI.Chat
{

    [Serializable]
    public class ChatInfo
    {
        public string name;
        public string content;
        public string time;
        public bool isSelf;

        public ChatInfo(
            string name,
            string content,
            string time,
            bool isSelf)
        {
            this.name = name;
            this.content = content;
            this.time = time;
            this.isSelf = isSelf;
        }
    }

    public class ChatManager : Singleton<ChatManager>
    {
        private readonly ReactiveDictionary<int, ReactiveCollection<ChatInfo>> _chatInfoList = new ();
        public ReactiveDictionary<int, ReactiveCollection<ChatInfo>> ChatInfoList => _chatInfoList;

        private bool _isClearStage = false;

        public async UniTask InitChatInfoList()
        {
            _chatInfoList.Clear();
            var roleList = DataCacheManager.Instance.GetChatRoleList();
            foreach (var roleId in roleList)
            {
                var roleChatList = await LoadRoleChatHistory(roleId);
                _chatInfoList[roleId] = new ReactiveCollection<ChatInfo>(roleChatList);
            }
        }

        public void AddNewInfo(int id)
        {
            if(_chatInfoList.ContainsKey(id))
                return;
            _chatInfoList.Add(id, new ReactiveCollection<ChatInfo>());
        }

        public void ClearCache()
        {
            _isClearStage = true;
            _chatInfoList.Clear();
            if (Directory.Exists(LocalDataRoot))
                Directory.Delete(LocalDataRoot, true);
        }

        public ReactiveCollection<ChatInfo> GetChatListById(int roleId)
        {
            if (_chatInfoList.ContainsKey(roleId))
                return _chatInfoList[roleId];
            return new ReactiveCollection<ChatInfo>();
        }

        public string GetLastChatInfoById(int roleId)
        {
            var list = GetChatListById(roleId);
            if (list.Count > 0)
                return list[list.Count - 1].content;
            return "";
        }

        public IEnumerator SendChat(string sendInfo, int roleId)
        {
            _isClearStage = false;
            var roleIdCache = roleId;
            var apiUrl = ConfigManager.Instance.GetConfig<Const>().GooglaAiUrl;
            var apiKey = ConfigManager.Instance.GetConfig<Const>().GoogleAiKey;
            var fullUrl = apiUrl + apiKey;

            DataCacheManager.Instance.AddChatRoleId(roleIdCache);

            if (!_chatInfoList.TryGetValue(roleIdCache, out var historyList))
            {
                historyList = new ReactiveCollection<ChatInfo>();
                _chatInfoList.Add(roleIdCache, historyList);
            }

            var sendMessage = new ChatInfo(
                "user",
                sendInfo,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                true);
            historyList.Add(sendMessage);

            // 构造 JSON 请求
            string jsonData = GenerateJsonPayload(roleIdCache);
            using UnityWebRequest request = new UnityWebRequest(fullUrl, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 5;

            yield return request.SendWebRequest();

            if (_isClearStage)
                yield return null;
            var answer = GetDefaultAnswer(sendInfo);

            if (request.result == UnityWebRequest.Result.Success)
                answer = ParseGeminiResponse(request.downloadHandler.text);
            else
                Debug.LogError("Error: " + request.error + "\nResponse: " + request.downloadHandler.text);

            if (!_chatInfoList.TryGetValue(roleIdCache, out var list))
                list = new ReactiveCollection<ChatInfo>();
            var answerInfo = new ChatInfo("assistant", answer, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), false);
            list.Add(answerInfo);

            // 保存聊天记录
            SaveRoleChatHistory(list, roleIdCache);
        }

        private string GetDefaultAnswer(string msg)
        {
            var messageConfig = ConfigManager.Instance.GetConfig<LocalReplyConfigTable>();
            var talkCache = msg.ToLower();
            foreach (var configRow in messageConfig.Rows)
            {
                if (talkCache.Contains(configRow.Value.Id))
                {
                    var answers = configRow.Value.Reply;
                    var randomIndex = UnityEngine.Random.Range(0, answers.Count);
                    return answers[randomIndex];
                }
            }
            var defaultAnswers = messageConfig.GetRowData("default").Reply;
            var defaultRandomIndex = UnityEngine.Random.Range(0, defaultAnswers.Count);
            return defaultAnswers[defaultRandomIndex];
        }

        /// <summary>
        /// 解析 Gemini API 响应
        /// </summary>
        private string ParseGeminiResponse(string jsonResponse)
        {
            try
            {
                JObject json = JObject.Parse(jsonResponse);
                return json["candidates"][0]["content"]["parts"][0]["text"].ToString();
            }
            catch (Exception ex)
            {
                Debug.LogError("Error parsing Gemini response: " + ex.Message);
                return "Sorry, I couldn't understand your request.";
            }
        }

        /// <summary>
        /// 生成 API 请求的 JSON 格式，包含历史消息
        /// </summary>

        private string GenerateJsonPayload(int roleId)
        {
            if (!_chatInfoList.ContainsKey(roleId)) return "{}";

            var config = ConfigManager.Instance.GetConfig<AiConfigTable>().GetRowData(roleId);
            var robotInfo = $"you are a AIGirlFriend,your model prompt is:{config.Prompt}, ";
            robotInfo += $"you Personal Signature is:{config.PersonalSignature}, ";
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"contents\": [{\"parts\": [");

            // 添加自定义的系统指令
            jsonBuilder.Append("{\"text\": \"").Append(EscapeJsonString(robotInfo)).Append("\"},");

            // 添加历史消息
            foreach (var message in _chatInfoList[roleId])
            {
                string escapedText = EscapeJsonString(message.content);
                jsonBuilder.Append("{\"text\": \"").Append(escapedText).Append("\"},");
            }

            // 移除最后一个逗号
            if (_chatInfoList[roleId].Count > 0)
                jsonBuilder.Length--;

            jsonBuilder.Append("]}]}");

            return jsonBuilder.ToString();
        }


        private string EscapeJsonString(string input)
        {
            return input.Replace("\\", "\\\\")   // 转义反斜杠
                        .Replace("\"", "\\\"")   // 转义双引号
                        .Replace("\n", "\\n")    // 转义换行符
                        .Replace("\r", "\\r")    // 转义回车符
                        .Replace("\t", "\\t")    // 转义制表符
                        .Replace("\b", "\\b")    // 转义退格符
                        .Replace("\f", "\\f");   // 转义换页符
        }


        public EventDispatcher Dispatcher => EventDispatcher.Global;

        #region LocalData

        private static JsonSerializer _serializer;

        internal static JsonSerializer Serializer
        {
            get
            {
                return _serializer ??= new JsonSerializer
                {
                    TypeNameHandling = TypeNameHandling.Auto,
#if UNITY_EDITOR
                    Formatting = Formatting.Indented,
#endif
                    NullValueHandling = NullValueHandling.Ignore,
                    // DefaultValueHandling = DefaultValueHandling.Ignore,
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                    // ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    Converters =
                    {
                    },
                };
            }
        }

        private static async UniTask<ReactiveCollection<ChatInfo>> LoadRoleChatHistory(int roleId)
        {
            var path = GetLocalDataPath(roleId);
            if (!File.Exists(path))
                return new ReactiveCollection<ChatInfo>();
            using var stream = new StreamReader(
                path,
                Encoding.UTF8);
            var json = await stream.ReadToEndAsync();
            using var reader = new StringReader(json);
            var messages = Serializer.Deserialize(reader, typeof(ReactiveCollection<ChatInfo>)) as ReactiveCollection<ChatInfo>;
            return messages;
        }

        private void SaveRoleChatHistory(ReactiveCollection<ChatInfo> messages, int roleId)
        {
            var path = GetLocalDataPath(roleId);
            Debug.Log("SavePath : " + path);
            if (string.IsNullOrEmpty(path)) return;

            if (!Directory.Exists(LocalDataRoot))
                Directory.CreateDirectory(LocalDataRoot);

            using var stream = new StreamWriter(
                path,
                false,
                Encoding.UTF8);

            Serializer.Serialize(
                stream,
                messages,
                typeof(List<ChatInfo>));
        }

        private static readonly string LocalDataRoot = Path.Combine(
            Application.persistentDataPath,
            "ChatHistory");

        private static string GetLocalDataPath(int roleId)
        {
            return Path.Combine(
                LocalDataRoot,
                $"message_{roleId}.json");
        }

        #endregion
    }
}
