using Core.Components;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.TargetSelect.Strategys;

namespace HotUpdate.Game.Battle.Skill.Component
{
    /// <summary>
    /// 玩家技能组件
    /// </summary>
    [ComponentId(typeof(PlayerSkillComponent))]
    public class PlayerSkillComponent : SkillComponent
    {
        public bool IsTrigger { get; set; }
        public bool IsRelease { get; set; }
        
        public override void BattleInit(IBattleEntityObject battleEntity)
        {
            base.BattleInit(battleEntity);
            
            var condition = castSkillConditionFactory.GetCastSkillCondition<PlayerDefaultCastSkillCondition>();
            AddCastCondition(condition);
            var strategy = targetSelectStrategyFactory.GetTargetSelectStrategy<PlayerBaseTargetSelectStrategy>();
            AddTargetSelectStrategy(strategy);
        }

        /// <summary>
        /// 释放终结技
        /// 点击终结技技能按键后，调用该方法改变标识，触发终结技释放
        /// </summary>
        public void ReleaseUltimate()
        {
            IsRelease = true;
        }
    }
}
