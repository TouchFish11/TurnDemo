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
        }

        protected override IEnumerator OnExceuteAction()
        {
            // 等待玩家行动结束
            yield return new WaitWhile(() => CanAct);
        }

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="triggerSkillEvent"></param>
        protected virtual void OnCastSkill(PlayerTriggerSkillEvent triggerSkillEvent)
        {
            if ((Object)triggerSkillEvent.BattleEntity != this)
            {
                return;
            }

            CastSkill(triggerSkillEvent.SkillId);
        }
    }
}
