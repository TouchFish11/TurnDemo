using System.Collections;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Skill.Handler
{
    /// <summary>
    /// 技能释放后处理器
    /// </summary>
    public abstract class SkillCastPostHandler : ISkillCastPostHandler
    {
        protected SkillContext SkillContext { get; private set; }
        
        protected IBattleContext BattleContext { get; private set; }
        
        public IEnumerator Handle(SkillContext skillContext)
        {
            SkillContext = skillContext;
            BattleContext = skillContext.Caster.Context;
            skillContext.Caster.Acting = false;
            yield return OnHandle();
        }

        protected virtual IEnumerator OnHandle()
        {
            yield break;
        }
    }
}
