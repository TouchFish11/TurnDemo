using System;
using System.Collections.Generic;
using Core.HotUpdate;
using Core.Log;
using HotUpdate.Base.Utility;

namespace HotUpdate.Game.Battle.Operation
{
    /// <summary>
    /// 技能按键UI数据提供器工厂
    /// </summary>
    public class SkillKeyUIDataProviderFactory : ISkillKeyUIDataProviderFactory
    {
        private readonly Dictionary<Type, ISkillKeyUIDataProvider> typeToInterfaceMap = new();
        
        private SkillKeyUIDataProviderFactory(IHotUpdateManager hotUpdateManager)
        {
            FactoryUtility.ScanAllType(typeToInterfaceMap, hotUpdateManager.GetHotAssemblies());
        }
        
        public ISkillKeyUIDataProvider GetProvider<TProvider>()where TProvider : class, ISkillKeyUIDataProvider
        {
            if (typeToInterfaceMap.TryGetValue(typeof(TProvider), out var targetSelectStrategy))
            {
                return targetSelectStrategy;
            }
            
            Logger.LogError(ELogTags.Battle, $"未找到按键UI数据提供器，{typeof(TProvider)}");
            return null;
        }
    }
}
