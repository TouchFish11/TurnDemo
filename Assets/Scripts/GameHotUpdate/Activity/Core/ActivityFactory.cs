using System;
using System.Collections.Generic;
using Core.Reflection;
using Core.Utility;

namespace GameHotUpdate.Activity.Core
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
            FactoryUtility.ScanAllType<IActivity, string, Type>(_activityDic, type => type.Name, type => type, assemblies: AssemblyUtility.GetHotUpdateAssemblies());
        }

        public Type GetActivity(string activityKey)
        {
            return _activityDic.GetValueOrDefault(activityKey);
        }
    }
}
