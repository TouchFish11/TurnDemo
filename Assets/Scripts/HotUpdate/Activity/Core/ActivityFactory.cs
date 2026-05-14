using System;
using System.Collections.Generic;
using Core.DI;
using Core.HotUpdate;
using Core.Reflection;
using HotUpdate.Base.Activity;

namespace HotUpdate.Activity.Core
{
    /// <summary>
    /// 活动工厂
    /// </summary>
    public class ActivityFactory : IActivityFactory
    {
        // 活动名称到活动类型的映射
        private readonly Dictionary<string, Type> _activityDic = new();
        
        public void InitFactory()
        {
            FactoryUtility.ScanAllType<IActivity, string, Type>(_activityDic, type => type.Name, type => type, assemblies: DIContainer.GetInstance<IHotUpdateManager>().GetHotAssemblies());
        }

        public Type GetActivity(string activityKey)
        {
            return _activityDic.GetValueOrDefault(activityKey);
        }
    }
}
