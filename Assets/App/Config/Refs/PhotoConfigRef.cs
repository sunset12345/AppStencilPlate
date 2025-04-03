using System.Collections.Generic;
using App.Config;

public class PhotoConfigRef : IConfigRef
{
    private Dictionary<int, List<int>> _aiID_photoList = new();

    public void Init(ConfigManager configManager){}

    public void PostProcessData(ConfigManager configManager)
    {
        _aiID_photoList.Clear();
        var photoConfigTable = configManager.GetConfig<PhotoConfigTable>();
        var photoList = photoConfigTable.Rows;
        foreach (var photoConfig in photoList)
        {
            if (!_aiID_photoList.ContainsKey(photoConfig.Value.AiId))
                _aiID_photoList.Add(photoConfig.Value.AiId, new List<int>());
            _aiID_photoList[photoConfig.Value.AiId].Add(photoConfig.Key);
        }
    }

    public List<int> GetPhotoList(int aiId) => _aiID_photoList[aiId];
}
