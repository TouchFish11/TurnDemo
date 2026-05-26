using Core.DI;
using Core.HotUpdate;
using Core.Log;
using HotUpdate.Base.Factory;

namespace HotUpdate.Game.Battle.UI
{
    /// <summary>
    /// 技能按键UI数据提供器工厂
    /// </summary>
    public class SkillKeyUIDataProviderFactory : Factory<ISkillKeyUIDataProvider>, ISkillKeyUIDataProviderFactory
    {
        void IFactory.InitFactory()
        {
            FactoryUtility.ScanAllType(typeToInterfaceMap, DIContainer.GetInstance<IHotUpdateManager>().GetAssemblies());
        }
        
        public ISkillKeyUIDataProvider GetCastSkillCondition<TProvider>()where TProvider : class, ISkillKeyUIDataProvider
        {
            if (typeToInterfaceMap.TryGetValue(typeof(TProvider), out var targetSelectStrategy))
            {
                return targetSelectStrategy;
            }
            
            Logger.LogError($"未找到按键UI数据提供器，{typeof(TProvider)}");
            return null;
        }
    }
}
