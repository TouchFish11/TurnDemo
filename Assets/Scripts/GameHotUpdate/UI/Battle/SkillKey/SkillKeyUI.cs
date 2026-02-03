using Core.Reflection;
using Core.Service;
using Core.UI;
using Core.Utility;
using Game.Battle;
using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Skill.Enum;
using Game.Battle.TargetSelect;
using Game.Tasks;
using GameHotUpdate.Battle.Event.UI;
using GameHotUpdate.Battle.TargetSelect.Strategys;
using GameHotUpdate.Skill.Component;
using GameHotUpdate.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameHotUpdate.UI.Battle.SkillKey
{
    /// <summary>
    /// ���ܰ���UI
    /// </summary>
    public class SkillKeyUI : BaseUIBehaviour
    {
        /// <summary>
        /// ���������׶�
        /// </summary>
        private enum E_TriggerPhase
        {
            /// <summary>
            /// δѡ��
            /// </summary>
            NonSeleceted,
            /// <summary>
            /// ��ѡ��
            /// </summary>
            Selected,
            /// <summary>
            /// ����
            /// </summary>
            Trigger,
        }

        [Inject] private TextMeshProUGUI txtSkillTip;

        private Toggle togSkillKeyUI;
        // ѡ��ʱ�����ű���
        private readonly Vector3 SelectedScale = Vector3.one * 1.3f;
        // ����ID
        private int skillId;
        // �ܷ񴥷�����
        private E_TriggerPhase triggerPhase = E_TriggerPhase.NonSeleceted;
        // ս�������Ľӿ�
        private IBattleContext battleContext;
        // ս��ʵ��ӿ�
        private IBattleEntityObject battleEntity;
        // ��ǰ��������
        private E_SkillType _SkillType;
        // 
        private ITargetSelectStrategy _targetSelectStrategy;

        protected override void Awake()
        {
            base.Awake();
            // Toggle�ͽű���ͬһ�������ϣ�������ͨ�����ֲ��ң������޷�����ͬ���ֶ�
            togSkillKeyUI = binder.GetControl<Toggle>(gameObject.name);

            UIUtility.AddCustomEventListener(this, EventTriggerType.PointerClick, OnClick);
            battleContext = ServiceLocator.Get<IBattleManager>().GetContext();
        }
        
        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="skillInfo"></param>
        /// <param name="group"></param>
        /// <param name="battleEntity"></param>
        public void Init(SkillInfo skillInfo, ToggleGroup group, IBattleEntityObject battleEntity)
        {
            skillId = skillInfo.f_id;
            togSkillKeyUI.group = group;
            this.battleEntity = battleEntity;
            txtSkillTip.text = skillInfo.f_skillRangeType.ToSkillRangeTypeText();

            var targetSelectStrategy = ServiceLocator.Get<IFactoryManager>()
                .GetFactory<ITargetSelectStrategyFactory, TargetSelectStrategyFactory>()
                .GetTargetSelectStrategy<PlayerBaseTargetSelectStrategy>();
            SetTargetSelectStrategy(targetSelectStrategy);

            // TODO����ʱֱ���жϣ���������
            _SkillType = (E_SkillType)skillInfo.f_SkillType;
            if (_SkillType is E_SkillType.NormalAttack or E_SkillType.UltimateSkill)
            {
                // ��������Ĭ��ѡ��
                DefaultSelect();
            }
        }

        /// <summary>
        /// Ĭ��ѡ��
        /// </summary>
        public void DefaultSelect()
        {
            togSkillKeyUI.isOn = true;
        }

        public void SetTargetSelectStrategy(ITargetSelectStrategy strategy)
        {
            _targetSelectStrategy = strategy;
        }

        protected override void OnToggleValueChanged(string togName, bool isOn)
        {
            OnSelected(isOn);
        }

        private void OnSelected(bool isOn)
        {
            if (isOn)
            {
                if (triggerPhase == E_TriggerPhase.Selected)
                {
                    triggerPhase = E_TriggerPhase.Trigger;
                }
                else
                {
                    // ѡ�У��Ŵ�+���ΪSelected
                    transform.localScale = SelectedScale;
                    triggerPhase = E_TriggerPhase.Selected;
                    battleContext?.GetEventBus().TriggerEvent(new SelectSkillEvent(battleContext, skillId, battleEntity, _targetSelectStrategy));
                }
            }
            else
            {
                // ȡ��ѡ�У����ŵ�1��+���ΪNonSeleceted
                transform.localScale = Vector3.one;
                triggerPhase = E_TriggerPhase.NonSeleceted;
            }
        }

        private void OnClick(BaseEventData baseEventData)
        {
            if (triggerPhase == E_TriggerPhase.Trigger && _SkillType != E_SkillType.UltimateSkill)
            {
                triggerPhase = E_TriggerPhase.Selected;
                // ִ�д��������¼�
                battleContext.GetEventBus().TriggerEvent(new PlayerTriggerSkillEvent(battleContext, skillId, battleEntity));
            }
            else
            {
                // �ͷ��սἼ
                // TODO����ʱֱ�ӵ��ã������Ż�
                battleEntity.GetComponent<PlayerSkillComponent>().ReleaseUltimate();
            }
        }

        /// <summary>
        /// ����״̬
        /// </summary>
        private void ResetState()
        {
            togSkillKeyUI.group = null;
            // ����Toggle״̬
            togSkillKeyUI.isOn = false;
            // �����߼�״̬����Toggleǿ�󶨣�
            triggerPhase = E_TriggerPhase.NonSeleceted;
            // �����Ӿ�״̬
            transform.localScale = Vector3.one;
            battleEntity = null;
        }

        protected override void OnDisable()
        {
            ResetState();
        }
    }
}
