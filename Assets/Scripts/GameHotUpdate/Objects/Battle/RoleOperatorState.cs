using System.Collections;
using Game.Battle.Objects;
using GameHotUpdate.Battle.Event.UI;
using UnityEngine;

namespace GameHotUpdate.Objects.Battle
{
    public class RoleOperatorState : OperatorState
    {
        public RoleOperatorState(IBattleEntityObject battleEntity) : base(battleEntity)
        {
            
        }

        public override void Enter()
        {
            // 监听技能释放事件
            BattleEntity.Context.GetEventBus().AddListener<RoleTriggerSkillEvent>(OnCastSkill);
            BattleEntity.Context.GetEventBus().AddListener<RoleTriggerUltimateSkillEvent>(OnCastUltimateSkill);
            base.Enter();
        }
        
        protected override IEnumerator OnExceuteAction()
        {
            // TODO：玩家自动逻辑预留
            bool isAuto = false;

            while (BattleEntity.CanAct)
            {
                if (!isAuto)
                {
                    yield return null;
                }
                else
                {
                    // 执行每个角色自己的自动选择技能策略
                    
                    yield break;
                }
            }
        }
        
        /// <summary>
        /// 释放技能事件回调
        /// 点击技能按键后触发
        /// </summary>
        /// <param name="triggerSkillEvent"></param>
        private void OnCastSkill(RoleTriggerSkillEvent triggerSkillEvent)
        {
            if ((Object)triggerSkillEvent.Caster != this.BattleEntity)
            {
                return;
            }

            BattleEntity.CastSkill(triggerSkillEvent.SkillId);
            // 行动结束
            // BattleEntity.CanAct = false;
        }

        /// <summary>
        /// 释放终结技技能事件回调
        /// 点击终结技技能按键后触发
        /// </summary>
        /// <param name="roleTriggerUltimateSkillEvent"></param>
        protected void OnCastUltimateSkill(RoleTriggerUltimateSkillEvent roleTriggerUltimateSkillEvent)
        {
            if ((BattleObject)roleTriggerUltimateSkillEvent.Caster != this.BattleEntity)
            {
                return;
            }

            BattleEntity.CastSkill(roleTriggerUltimateSkillEvent.SkillId);
        }

        public override void Execute()
        {

        }

        public override void Exit()
        {
            // 移除事件监听
            BattleEntity.Context.GetEventBus().RemoveListener<RoleTriggerSkillEvent>(OnCastSkill);
            BattleEntity.Context.GetEventBus().RemoveListener<RoleTriggerUltimateSkillEvent>(OnCastUltimateSkill);
        }
    }
}
