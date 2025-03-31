using DFDev.EventSystem;
using DFDev.Singleton;
using UnityEngine;

namespace App.DataCache
{
    /// <summary>
    /// 数据缓存管理器
    /// </summary>
    /// <remarks>
    /// 该类用于管理应用程序中的数据缓存。它允许设置、获取和清除缓存数据。
    /// </remarks>
    public class DataCacheManager : Singleton<DataCacheManager>,IEventSender
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

        public void DeleteAccount()
        {
            PlayerPrefs.DeleteAll();
            // 触发数据清除事件
            this.DispatchEvent(Witness<DataCacheClearAllEvent>._);
        }
    }

    public class DataCacheClearAllEvent : EventBase{ }
    public class DataCahceUpdateEvent : EventBase<string>
    { 
        public string Key => Field1;
    }

}