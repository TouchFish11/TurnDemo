using System.Collections;
using HotUpdate.Battle.Event.UI;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Battle.Object.StateMeachine
{
    /// <summary>
    /// 操作状态
    /// </summary>
    public class OperatorState : TurnState
    {
        public OperatorState(IBattleEntityObject battleEntity) : base(battleEntity)
        { 

        }

        public override void Enter()
        {
            // 监听技能释放事件
            PlayerObject.Context.GetEventBus().AddListener<RoleTriggerSkillEvent>(OnCastSkill);
            PlayerObject.Context.GetEventBus().AddListener<RoleTriggerUltimateSkillEvent>(OnCastUltimateSkill);
            PlayerObject.StartCoroutine(OnExceuteAction());
        }

        private IEnumerator OnExceuteAction()
        {
            // TODO：玩家自动逻辑预留
            bool isAuto = false;

            while (PlayerObject.CanAct)
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
            if ((UnityEngine.Object)triggerSkillEvent.Caster != PlayerObject)
            {
                return;
            }

            PlayerObject.CastSkill(triggerSkillEvent.SkillId);
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
            if ((BattleObject)roleTriggerUltimateSkillEvent.Caster != PlayerObject)
            {
                return;
            }

            PlayerObject.CastSkill(roleTriggerUltimateSkillEvent.SkillId);
        }

        public override void Exit()
        {
            // 移除事件监听
            PlayerObject.Context.GetEventBus().RemoveListener<RoleTriggerSkillEvent>(OnCastSkill);
            PlayerObject.Context.GetEventBus().RemoveListener<RoleTriggerUltimateSkillEvent>(OnCastUltimateSkill);
        }
    }
}
