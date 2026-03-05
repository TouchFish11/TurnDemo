using System;
using System.Collections.Generic;
using System.Reflection;
using Core.HotUpdate;
using Core.Log;
using Core.Reflection;
using Core.Service;
using HotUpdate.Activity.Data;

namespace HotUpdate.Activity.Core
{
    /// <summary>
    /// 活动数据工厂
    /// </summary>
    public class ActivityDataFactory : IActivityDataFactory
    {
        private readonly IHotUpdateManager _hotUpdateManager = ServiceLocator.Get<IHotUpdateManager>();
        private readonly Dictionary<int, ActivityData> _data = new();
        
        public void InitFactory()
        {
            FactoryUtility.ScanAllType<ActivityData, int, ActivityData>(_data, KeyFunc, ValueFunc,
                assemblies: _hotUpdateManager.GetHotAssemblies());
        }

        private static ActivityData ValueFunc(Type type)
        {
            return (ActivityData)Activator.CreateInstance(type);
        }

        private static int KeyFunc(Type type)
        {
            var activityIdAttribute = type.GetCustomAttribute<ActivityIdAttribute>();
            if (activityIdAttribute != null)
            {
                return activityIdAttribute.ActivityId;
            }

            LogManager.LogError($"{nameof(ActivityDataFactory)}.{nameof(KeyFunc)}：{type.FullName}不存在特性：{nameof(ActivityIdAttribute)}");
            return -1;
        }

        public ActivityData GetData(int activityId)
        {
            return _data.GetValueOrDefault(activityId);
        }
    }
}
