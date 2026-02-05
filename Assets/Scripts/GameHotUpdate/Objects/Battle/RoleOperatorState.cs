using System.Collections;
using Core.Log;
using Game.Battle.Objects;
using Game.Battle.Skill.Component;
using GameHotUpdate.Battle.Event.UI;
using GameHotUpdate.Skill.Component;
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
            BattleEntity.Context.GetEventBus().AddListener<PlayerTriggerSkillEvent>(OnCastSkill);
            BattleEntity.Context.GetEventBus().AddListener<PlayerTriggerUltimateSkillEvent>(OnCastUltimateSkill);
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
        private void OnCastSkill(PlayerTriggerSkillEvent triggerSkillEvent)
        {
            if ((Object)triggerSkillEvent.Caster != this.BattleEntity)
            {
                return;
            }

            BattleEntity.GetComponent<SkillComponent>().CastSkill(triggerSkillEvent.SkillId);
            // // 行动结束
            // BattleEntity.CanAct = false;
            // LogManager.Log($"行动结束");
        }

        /// <summary>
        /// 释放终结技技能事件回调
        /// 点击终结技技能按键后触发
        /// </summary>
        /// <param name="playerTriggerUltimateSkillEvent"></param>
        protected void OnCastUltimateSkill(PlayerTriggerUltimateSkillEvent playerTriggerUltimateSkillEvent)
        {
            if ((BattleObject)playerTriggerUltimateSkillEvent.Caster != this.BattleEntity)
            {
                return;
            }

            BattleEntity.GetComponent<PlayerSkillComponent>().CastSkill(playerTriggerUltimateSkillEvent.SkillId);
        }

        public override void Execute()
        {

        }

        public override void Exit()
        {
            // 移除事件监听
            BattleEntity.Context.GetEventBus().RemoveListener<PlayerTriggerSkillEvent>(OnCastSkill);
            BattleEntity.Context.GetEventBus().RemoveListener<PlayerTriggerUltimateSkillEvent>(OnCastUltimateSkill);
        }
    }
}
