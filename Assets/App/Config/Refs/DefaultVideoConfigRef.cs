using System.Collections.Generic;
using App.Config;

public class DefaultVideoConfigRef : IConfigRef
{
    private readonly Dictionary<int, List<DefaultVideoConfig>> _videoDictionary = new();

    public int GetVideoCount(int girlId) => _videoDictionary.ContainsKey(girlId) ? _videoDictionary[girlId].Count : 0;

    public List<DefaultVideoConfig> GetVideoList(int girlId) => _videoDictionary.ContainsKey(girlId) ? _videoDictionary[girlId] : null;

    public void Init(ConfigManager configManager) { }

    public void PostProcessData(ConfigManager configManager)
    {
        _videoDictionary.Clear();
        var videoConfigTable = configManager.GetConfig<DefaultVideoConfigTable>();
        foreach (var videoConfig in videoConfigTable.Rows)
        {
            var girlId = videoConfig.Value.AiId;
            if (_videoDictionary.TryGetValue(girlId, out List<DefaultVideoConfig> list))
                list.Add(videoConfig.Value);
            else
                _videoDictionary.Add(girlId, new List<DefaultVideoConfig> { videoConfig.Value });
        }
    }
}
