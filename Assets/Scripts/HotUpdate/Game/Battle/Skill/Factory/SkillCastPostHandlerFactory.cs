using Core.HotUpdate;
using Core.Log;
using HotUpdate.Base.Factory;
using HotUpdate.Game.Battle.Skill.Handler;

namespace HotUpdate.Game.Battle.Skill.Factory
{
    /// <summary>
    /// 技能释放后处理器工厂
    /// </summary>
    public class SkillCastPostHandlerFactory : Factory<ISkillCastPostHandler>, ISkillCastPostHandlerFactory
    {
        public SkillCastPostHandlerFactory(IHotUpdateManager hotUpdateManager) : base(hotUpdateManager)
        {
            
        }
        
        public ISkillCastPostHandler GetSkillCastPostHandler<THandler>()where THandler : class, ISkillCastPostHandler
        {
            if (typeToInterfaceMap.TryGetValue(typeof(THandler), out var targetSelectStrategy))
            {
                return targetSelectStrategy;
            }
            
            Logger.LogError($"未找到技能释放后处理器，{typeof(THandler)}");
            return null;
        }
    }
}
