using System.Collections;
using Core.DI;
using Core.Utility;
using HotUpdate.Game.Battle.Core;

namespace HotUpdate.Game.Battle.Skill.Base.Flow
{
    /// <summary>
    /// 技能阶段策略
    /// </summary>
    public abstract class SkillPhaseStrategy : ISkillPhaseStrategy
    {
        [Inject] protected BattleCoordinator battleCoordinator;
        // 技能对象
        protected ISkill skill;
        // 技能动画数组
        private string[] _animNames;

        /// <summary>
        /// 技能的最后一个动画的名称
        /// </summary>
        protected string LastAnimationName
        {
            get
            {
                _animNames ??= TextUtility.Split(SkillContext.SkillInfo.f_animNames, 2);
                return _animNames[_animNames.Length - 1];
            }
        }
        
        /// <summary>
        /// 技能动画数组
        /// </summary>
        protected string[] AnimNames => _animNames ??= TextUtility.Split(SkillContext.SkillInfo.f_animNames, 2);
        
        /// <summary>
        /// 技能上下文
        /// </summary>
        protected SkillContext SkillContext => skill.SkillContext;
        
        public void SetSkill(ISkill skill)
        {
            this.skill = skill;
        }

        public abstract IEnumerator Execute();
    }
}
