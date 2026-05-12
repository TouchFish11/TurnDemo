using System;
using System.Collections.Generic;
using System.Reflection;
using Core.DI;
using Core.HotUpdate;
using Core.Log;
using Core.Reflection;
using HotUpdate.Base.Activity;
using HotUpdate.Game.Activity.Data;

namespace HotUpdate.Game.Activity.Core
{
    /// <summary>
    /// 活动数据工厂
    /// </summary>
    public class ActivityDataFactory : IActivityDataFactory
    {
        private readonly IHotUpdateManager _hotUpdateManager = DIContainer.GetInstance<IHotUpdateManager>();
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

            Logger.LogError($"{nameof(ActivityDataFactory)}.{nameof(KeyFunc)}：{type.FullName}不存在特性：{nameof(ActivityIdAttribute)}");
            return -1;
        }

        public IActivityData GetData(int activityId)
        {
            return _data.GetValueOrDefault(activityId);
        }
    }
}
