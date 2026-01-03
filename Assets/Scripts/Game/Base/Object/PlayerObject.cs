using Framework;
using Game.Battle;
using System.Collections;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 玩家角色
    /// </summary>
    public class PlayerObject : BattleObject
    {
        /// <summary>
        /// 角色信息
        /// </summary>
        public RoleInfo RoleInfo { get; private set; }

        public override void BaseInit(int id)
        {
            base.BaseInit(id);
            RoleInfo = BinaryDataManager.Instance.GetConfig<RoleInfoContainer>(E_ConfigLoadType.Editor).dataDic[id];
        }

        public override void BattleInit(int battleEntityId, IBattleContext context)
        {
            base.BattleInit(battleEntityId, context);
            // 添加战斗相关组件
            AddComponents(TextUtility.Split(RoleInfo.f_comNames, 2));
            // 监听玩家技能触发事件
            Context.GetEventBus().AddListener<PlayerTriggerSkillEvent>(OnCastSkill);
            // 监听玩家终结技触发事件
            Context.GetEventBus().AddListener<PlayerTriggerUltimateSkillEvent>(OnCastUltimateSkill);
        }

        protected override IEnumerator OnExceuteAction()
        {
            // 等待玩家行动结束
            yield return new WaitWhile(() => CanAct);
        }

        protected override void OnPreTakeDamage(DamageResult damageResult)
        {
            /*暂时不需要实现*/
        }

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="triggerSkillEvent"></param>
        protected virtual void OnCastSkill(PlayerTriggerSkillEvent triggerSkillEvent)
        {
            if ((Object)triggerSkillEvent.Caster != this)
            {
                return;
            }

            CastSkill(triggerSkillEvent.SkillId);
        }

        /// <summary>
        /// 释放终结技
        /// </summary>
        /// <param name="playerTriggerUltimateSkillEvent"></param>
        protected virtual void OnCastUltimateSkill(PlayerTriggerUltimateSkillEvent playerTriggerUltimateSkillEvent)
        {
            this.GetComponent<PlayerSkillComponent>().CastUltimateSkill(playerTriggerUltimateSkillEvent.SkillId);
        }
    }
}
