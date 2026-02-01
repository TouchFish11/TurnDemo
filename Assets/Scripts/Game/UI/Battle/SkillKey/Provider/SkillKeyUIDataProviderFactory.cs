using Core.Log;
using Core.Reflection;
using Core.Utility;

namespace Game.UI.Battle.SkillKey.Provider
{
    /// <summary>
    /// 技能按键UI数据提供器工厂
    /// </summary>
    public class SkillKeyUIDataProviderFactory : Factory<ISkillKeyUIDataProvider>, ISkillKeyUIDataProviderFactory
    {
        void IFactory.InitFactory()
        {
            FactoryUtility.ScanAllType(typeToInterfaceMap, AssemblyUtility.GetHotUpdateAssemblies());
        }
        
        public ISkillKeyUIDataProvider GetCastSkillCondition<TProvider>()where TProvider : class, ISkillKeyUIDataProvider
        {
            if (typeToInterfaceMap.TryGetValue(typeof(TProvider).ToIdentifier(), out var targetSelectStrategy))
            {
                return targetSelectStrategy;
            }
            
            LogManager.LogError($"未找到按键UI数据提供器，{typeof(TProvider).ToIdentifier()}");
            return null;
        }
    }
}
