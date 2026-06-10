using System.Collections.Generic;
using System.Reflection;
using Core.DI;
using Core.HotUpdate;
using Core.Log;
using HotUpdate.Base.Data;

namespace HotUpdate.Game.Activity.Core
{
    /// <summary>
    /// 活动数据工厂
    /// </summary>
    public class ActivityDataFactory : IActivityDataFactory
    {
        private readonly Dictionary<int, ActivityData> _datas = new();

        private ActivityDataFactory(IHotUpdateManager hotUpdateManager)
        {
            ScanActivityData(hotUpdateManager);
        }

        private void ScanActivityData(IHotUpdateManager hotUpdateManager)
        {
            foreach (var assembly in hotUpdateManager.GetHotAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeof(ActivityData).IsAssignableFrom(type) || type.IsAbstract) 
                        continue;
                    
                    var activityIdAttribute = type.GetCustomAttribute<ActivityIdAttribute>();
                    if (activityIdAttribute == null)
                    {
                        Logger.LogError($"{nameof(ActivityDataFactory)}: {type} does not have a marking {nameof(ActivityIdAttribute)}.");
                        continue;
                    }
                    
                    var data = DIContainer.Create(null, type) as ActivityData;
                    _datas.Add(activityIdAttribute.ActivityId, data);
                }
            }
        }

        public bool tryGetData(int activityId,  out ActivityData data)
        {
            return _datas.TryGetValue(activityId, out data);
        }
    }
}
