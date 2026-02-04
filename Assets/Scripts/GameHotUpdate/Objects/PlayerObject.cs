using System.Collections;
using Core.DataPersistence.Binary;
using Core.Service;
using Core.Utility;
using Game.Animation;
using Game.Battle.Context;
using GameHotUpdate.Animation;
using GameHotUpdate.Battle.Event.UI;
using GameHotUpdate.Battle.ResponsibilityChain.DamageChain;
using GameHotUpdate.Skill.Component;
using UnityEngine;

namespace GameHotUpdate.Objects
{
    /// <summary>
    /// ��ҽ�ɫ
    /// </summary>
    public abstract class PlayerObject : BattleObject
    {
        /// <summary>
        /// ��ɫ��Ϣ
        /// </summary>
        public RoleInfo RoleInfo { get; private set; }

        public override void BaseInit(int id)
        {
            base.BaseInit(id);
            RoleInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<RoleInfoContainer>(EConfigLoadType.Editor).dataDic[id];
        }

        public override void BattleInit(int battleEntityId, IBattleContext context)
        {
            base.BattleInit(battleEntityId, context);
            
            // 初始化伤害链
            damageChain = DamageChainBuilder.GetRolrDamageChain();
            // ����ս��������
            AddComponents(TextUtility.Split(RoleInfo.f_comNames, 2));
            // ������Ҽ��ܴ����¼�
            Context.GetEventBus().AddListener<PlayerTriggerSkillEvent>(OnCastSkill);
            // ��������սἼ�����¼�
            Context.GetEventBus().AddListener<PlayerTriggerUltimateSkillEvent>(OnCastUltimateSkill);
        }

        protected override IEnumerator OnExceuteAction()
        {
            // TODO：玩家自动逻辑预留
            bool isAuto = false;

            while (CanAct)
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
            
            // if (isAuto)
            // {
            //
            //     
            // }
            // else
            // {
            //     // �ȴ�����ж�����
            //     yield return new WaitWhile(() => CanAct);
            // }
        }

        /// <summary>
        /// �ͷż���
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
        /// ����սἼ�����¼��ص�
        /// </summary>
        /// <param name="playerTriggerUltimateSkillEvent"></param>
        protected virtual void OnCastUltimateSkill(PlayerTriggerUltimateSkillEvent playerTriggerUltimateSkillEvent)
        {
            if ((Object)playerTriggerUltimateSkillEvent.Caster != this)
            {
                return;
            }

            GetComponent<PlayerSkillComponent>().CastSkill(playerTriggerUltimateSkillEvent.SkillId);
        }

        public override IEnumerator Die()
        {
            // ��Ҳ�����������
            yield return ServiceLocator.Get<IAnimationPlayManager>().WaitForAnimOver(GetComponent<BattleAnimationComponent>(), AnimationComponent.Battle_Layer_Name, E_AnimationType.Death);
        }
    }
}
