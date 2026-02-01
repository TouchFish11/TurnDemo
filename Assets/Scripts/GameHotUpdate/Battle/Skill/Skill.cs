using System.Collections;
using System.Collections.Generic;
using Core.DataPersistence.Binary;
using Core.Service;
using Core.Utility;
using Game.Battle.Context;
using Game.Battle.Enum;
using Game.Battle.Objects;
using Game.Battle.Skill;
using Game.Battle.Skill.Handler;
using Game.Battle.Skill.Interface;
using Game.Battle.Status;
using Game.Battle.TargetSelect;
using Game.Property;
using Game.UI.Battle;
using Game.VFX;
using GameHotUpdate.Property;
using GameHotUpdate.UI.Battle.Base;
using UnityEngine;

namespace GameHotUpdate.Battle.Skill
{
    /// <summary>
    /// ���ܻ���
    /// </summary>
    public abstract class Skill : ISkill
    {
        // ����������
        protected ProjectileData projectileData;
        // ������Transform
        protected ProjectileTrans projectileTrans;
        // ��Ч��Ϣ
        protected VFXInfo vFXInfo;
        // buffId����
        protected int[] statusIds;
        private readonly float waitTime = 0.85f;

        public SkillInfo SkillInfo { get; private set; }

        public IBattleEntityObject Caster { get; private set; }

        public IBattleEntityObject MainTarget { get; private set; }

        public List<IBattleEntityObject> AllTargets { get; private set; }

        public IPropertyComponent PropertyComponent { get; private set; }

        public ISkillCastPostHandler SkillCastPostHandler { get; private set; }

        public IStatusAddStrategy StatusAddStrategy { get; private set; }

        public ITargetSelectStrategy TargetSelectStrategy { get; private set; }

        protected Skill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy)
        {
            Caster = caster;
            SkillInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<SkillInfoContainer>(EConfigLoadType.Editor).dataDic[skillId];
            SkillCastPostHandler = postHandler;
            statusIds = TextUtility.SplitToIntArr(SkillInfo.f_statusId, 2);
            StatusAddStrategy = statusAddStrategy;
            PropertyComponent = Caster.GetComponent<PropertyComponent>();
        }

        public virtual void Init(IBattleEntityObject mainTarget, List<IBattleEntityObject> allTargets)
        {
            MainTarget = mainTarget;
            AllTargets = allTargets;
        }

        public void SetTargetSelectStrategy(ITargetSelectStrategy targetSelectStrategy)
        {
            TargetSelectStrategy = targetSelectStrategy;
        }

        /// <summary>
        /// �����ͷ�ǰ
        /// ����Ŀ��ѡ�񡢳�ʼ������Ŀ��
        /// �ȵ��ø����鷽��
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnPreCast(IBattleContext context)
        {
            // ѡ��Ŀ��
            ServiceLocator.Get<ITargetSelectManager>().SelectTarget(context, Caster, SkillInfo, TargetSelectStrategy);
            // ��ʼ������Ŀ��
            ServiceLocator.Get<ISkillManager>().InitSkillTarget(this);
        }

        public IEnumerator Cast(IBattleContext context)
        {
            // �����ͷ�ǰ
            OnPreCast(context);
            // ���������������
            yield return OnCast(context);
            // �ȴ�ʱ�䣬�Ż�ս������
            yield return new WaitForSeconds(waitTime);
            // �ͷŽ�������
            yield return OnPostCast();
        }

        /// <summary>
        /// �����ͷ�ʱ
        /// ���������������������
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract IEnumerator OnCast(IBattleContext context);

        /// <summary>
        /// �����ͷź�
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator OnPostCast()
        {
            // TODO�������ƶ���SkillCastPostHandler��
            // ���ս��������ʾ���˺����ı�
            ((BattleController)ServiceLocator.Get<IBattleUIScheduler>().BattleController).BattleUiManager.ClearActiveDamageTextUI();
            // ������˺��ۼ���ʾUI
            ((BattleController)ServiceLocator.Get<IBattleUIScheduler>().BattleController).BattleUiManager.UpdateCumulativeDamage(false, 0);

            yield return SkillCastPostHandler.OnHandle(this);
        }

        /// <summary>
        /// �����ͷŹ�����ָ�����
        /// ������ã�������˺���ʱ��ָ�����
        /// </summary>
        public virtual void RecoverEnergy()
        {
            int newValue = PropertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentEnergy);
            PropertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentEnergy, newValue + SkillInfo.f_recoveryEnergy);
        }
    }
}