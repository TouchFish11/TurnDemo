using Core.HotUpdate;
using Core.Log;
using Core.Reflection;
using Core.Service;
using Core.Utility;
using HotUpdate.Battle.Skill.Handler;
using HotUpdate.Core.Battle.Skill;

namespace HotUpdate.Battle.Skill.Factory
{
    /// <summary>
    /// 技能释放后处理器工厂
    /// </summary>
    public class SkillCastPostHandlerFactory : Factory<ISkillCastPostHandler>, ISkillCastPostHandlerFactory
    {
        /// <summary>
        /// 初始化工厂
        /// </summary>
        void IFactory.InitFactory()
        {
            FactoryUtility.ScanAllType(typeToInterfaceMap, ServiceLocator.Get<IHotUpdateManager>().GetAssemblies());
        }
        
        public ISkillCastPostHandler GetSkillCastPostHandler<THandler>()where THandler : class, ISkillCastPostHandler
        {
            if (typeToInterfaceMap.TryGetValue(typeof(THandler).ToIdentifier(), out var targetSelectStrategy))
            {
                return targetSelectStrategy;
            }
            
            LogManager.LogError($"未找到技能释放后处理器，{typeof(THandler).ToIdentifier()}");
            return null;
        }
    }
}
