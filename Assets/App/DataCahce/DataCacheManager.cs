using System.Collections.Generic;
using DFDev.EventSystem;
using DFDev.Singleton;
using Spine;
using UnityEngine;

namespace App.DataCache
{
    /// <summary>
    /// 数据缓存管理器
    /// </summary>
    /// <remarks>
    /// 该类用于管理应用程序中的数据缓存。它允许设置、获取和清除缓存数据。
    /// </remarks>
    public class DataCacheManager : Singleton<DataCacheManager>, IEventSender
    {
        public EventDispatcher Dispatcher => EventDispatcher.Global;

        public void SetData(string key, object value)
        {
            DataCache.Save(key, value);
            // 触发数据更新事件
            this.DispatchEvent(Witness<DataCahceUpdateEvent>._, key);
        }

        public void AddDiamond(int diamond)
        {
            var currentDiamond = DataCache.Load<int>(DataEnum.Diamond.ToString());
            currentDiamond += diamond;
            DataCache.Save(DataEnum.Diamond.ToString(), currentDiamond);
            // 触发数据更新事件
            this.DispatchEvent(Witness<DataCahceUpdateEvent>._, DataEnum.Diamond.ToString());
        }

        public List<int> GetUnlockList()
        {
            var unlockList = DataCache.Load<List<int>>(DataEnum.AiUnlock.ToString(), new List<int>());
            return unlockList;
        }

        public List<int> GetChatRoleList()
        {
            var unlockList = DataCache.Load<List<int>>(DataEnum.ChatRoleList.ToString(), new List<int>());
            return unlockList;
        }

        public void AddChatRoleId(int roleId)
        {
            var unlockList = DataCache.Load<List<int>>(DataEnum.ChatRoleList.ToString(), new List<int>());
            if (!unlockList.Contains(roleId))
            {
                unlockList.Add(roleId);
                SetData(DataEnum.ChatRoleList.ToString(), unlockList);
            }
        }

        public List<int> GetFollowList()
        {
            var followList = DataCache.Load<List<int>>(DataEnum.FollowList.ToString(), new List<int>());
            return followList;
        }

        public void ChangeFollowList(int aiId, bool isAdd)
        {
            var followList = DataCache.Load<List<int>>(DataEnum.FollowList.ToString(), new List<int>());
            if (isAdd)
            {
                if (followList.Contains(aiId))
                    return;
                followList.Add(aiId);
                SetData(DataEnum.FollowList.ToString(), followList);
            }
            else
            {
                if (!followList.Contains(aiId))
                    return;
                followList.Remove(aiId);
                SetData(DataEnum.FollowList.ToString(), followList);
            }
        }

        public void DeleteAccount()
        {
            PlayerPrefs.DeleteAll();
            // 触发数据清除事件
            this.DispatchEvent(Witness<DataCacheClearAllEvent>._);
        }

        public void SaveTimeLine(TimeLineData tiemLine)
        {
            var timeLineList = DataCache.Load<List<TimeLineData>>(DataEnum.TimeLine.ToString(), new List<TimeLineData>());
            timeLineList.Add(tiemLine);

            DataCache.Save(DataEnum.TimeLine.ToString(), timeLineList);
        }

        public bool IsPhotoLiked(int photoId)
        {
            var likePhotos = DataCache.Load<List<int>>(DataEnum.LikePhotos.ToString(), new List<int>());
            return likePhotos.Contains(photoId);
        }

        public void ChangeLikeStage(int photoId)
        {
            var likePhotos = DataCache.Load<List<int>>(DataEnum.LikePhotos.ToString(), new List<int>());
            if (!likePhotos.Contains(photoId))
                likePhotos.Add(photoId);
            else
                likePhotos.Remove(photoId);
            SetData(DataEnum.LikePhotos.ToString(), likePhotos);
        }
    }

    [SerializeField]
    public class TimeLineData
    {
        public string Name;
        public string TimeInfo;
        public string Description;
        public TimeLineData(
            string name,
            string time,
            string des
        )
        {
            Name = name;
            TimeInfo = time;
            Description = des;
        }
    }

    public class DataCacheClearAllEvent : EventBase { }
    public class DataCahceUpdateEvent : EventBase<string>
    {
        public string Key => Field1;
    }

}