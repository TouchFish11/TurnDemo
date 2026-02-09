using Core.Reflection;
using Core.Service;
using Core.UI;
using Core.Utility;
using Game.Battle;
using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Skill.Enum;
using Game.Battle.TargetSelect;
using GameHotUpdate.Battle.Event.UI;
using GameHotUpdate.Battle.TargetSelect.Strategys;
using GameHotUpdate.Skill.Component;
using GameHotUpdate.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameHotUpdate.Battle.UI.SkillKey
{
    /// <summary>
    /// 技能按键UI组件
    /// 负责单个技能按键的显示、选中、触发等交互逻辑
    /// </summary>
    public class SkillKeyUI : BaseUIBehaviour
    {
        /// <summary>
        /// 技能触发阶段
        /// 用于管控技能按键从"未选中→选中→触发"的状态流转
        /// </summary>
        private enum E_TriggerPhase
        {
            /// <summary>
            /// 未选中状态
            /// 初始/取消选中时的默认状态
            /// </summary>
            NonSeleceted,
            
            /// <summary>
            /// 已选中状态
            /// 按键被选中但未触发技能的状态
            /// </summary>
            Selected,
            
            /// <summary>
            /// 触发状态
            /// 选中后再次点击进入的触发状态
            /// </summary>
            Trigger,
        }

        // 技能提示文本（显示技能类型信息）
        [Inject] private TextMeshProUGUI txtSkillTip;

        /// <summary>
        /// 技能按键的Toggle组件（用于选中状态切换）
        /// </summary>
        private Toggle togSkillKeyUI;
        
        /// <summary>
        /// 选中时的缩放比例（视觉反馈）
        /// </summary>
        private readonly Vector3 SelectedScale = Vector3.one * 1.3f;
        
        /// <summary>
        /// 当前绑定的技能ID
        /// </summary>
        private int skillId;
        
        /// <summary>
        /// 当前技能触发阶段（状态机核心变量）
        /// </summary>
        private E_TriggerPhase triggerPhase = E_TriggerPhase.NonSeleceted;
        
        /// <summary>
        /// 战斗上下文（提供战斗环境、事件总线等核心能力）
        /// </summary>
        private IBattleContext battleContext;
        
        /// <summary>
        /// 绑定的战斗实体（当前操控的角色/单位）
        /// </summary>
        private IBattleEntityObject battleEntity;
        
        /// <summary>
        /// 当前技能类型（普攻/奥义等）
        /// </summary>
        private E_SkillType _SkillType;
        
        /// <summary>
        /// 目标选择策略（决定技能选中的目标规则）
        /// </summary>
        private ITargetSelectStrategy _targetSelectStrategy;

        /// <summary>
        /// 组件初始化（Awake生命周期）
        /// 初始化Toggle组件、注册事件、获取战斗上下文
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            // 获取当前GameObject同名的Toggle组件（UI绑定约定）
            // 注：Toggle和图片在同一GameObject下，需通过名称匹配，无法直接绑定同名字段
            togSkillKeyUI = binder.GetControl<Toggle>(gameObject.name);

            // 注册点击事件监听
            UIUtility.AddCustomEventListener(this, EventTriggerType.PointerClick, OnClick);
            // 从服务定位器获取战斗上下文
            battleContext = ServiceLocator.Get<IBattleManager>().GetContext();
        }

        /// <summary>
        /// 初始化技能按键数据
        /// </summary>
        /// <param name="skillInfo">技能配置信息</param>
        /// <param name="group">Toggle分组（用于互斥选中）</param>
        /// <param name="battleEntity">绑定的战斗实体（角色）</param>
        public void Init(SkillInfo skillInfo, ToggleGroup group, IBattleEntityObject battleEntity)
        {
            // 绑定技能ID
            skillId = skillInfo.f_id;
            // 设置Toggle分组（实现技能按键互斥选中）
            togSkillKeyUI.group = group;
            // 绑定战斗实体
            this.battleEntity = battleEntity;
            // 从工厂获取玩家基础目标选择策略（目标选择的规则逻辑）
            this._targetSelectStrategy = ServiceLocator.Get<IFactoryManager>()
                .GetFactory<ITargetSelectStrategyFactory, TargetSelectStrategyFactory>()
                .GetTargetSelectStrategy<PlayerBaseTargetSelectStrategy>();
            
            // 设置技能提示文本（显示技能类型）
            txtSkillTip.text = skillInfo.f_skillRangeType.ToSkillRangeTypeText();
            // 直接转换技能类型
            _SkillType = (E_SkillType)skillInfo.f_SkillType;
            // 普攻/终结技技能默认选中
            if (_SkillType is E_SkillType.NormalAttack or E_SkillType.UltimateSkill)
            {
                DefaultSelect();
            }
        }

        /// <summary>
        /// 设置技能按键为默认选中状态
        /// </summary>
        public void DefaultSelect()
        {
            togSkillKeyUI.isOn = true;
        }

        /// <summary>
        /// Toggle选中状态变更回调（BaseUIBehaviour生命周期）
        /// </summary>
        /// <param name="togName">Toggle组件名称</param>
        /// <param name="isOn">是否选中</param>
        protected override void OnToggleValueChanged(string togName, bool isOn)
        {
            OnSelected(isOn);
        }

        /// <summary>
        /// 处理技能选中状态变更
        /// </summary>
        /// <param name="isOn">是否选中</param>
        private void OnSelected(bool isOn)
        {
            if (isOn)
            {
                // 已选中→再次触发：切换为触发状态
                if (triggerPhase == E_TriggerPhase.Selected)
                {
                    triggerPhase = E_TriggerPhase.Trigger;
                }
                else
                {
                    // 未选中→选中：视觉缩放+状态更新+发送技能选中事件
                    transform.localScale = SelectedScale;
                    triggerPhase = E_TriggerPhase.Selected;
                    // 触发技能选中事件
                    battleContext?.GetEventBus().TriggerEvent(new SelectSkillEvent(battleContext, skillId, battleEntity, _targetSelectStrategy));
                }
            }
            else
            {
                // 取消选中：恢复缩放+重置状态
                transform.localScale = Vector3.one;
                triggerPhase = E_TriggerPhase.NonSeleceted;
            }
        }

        /// <summary>
        /// 技能按键点击事件处理
        /// </summary>
        /// <param name="baseEventData">事件数据（UI事件基础数据）</param>
        private void OnClick(BaseEventData baseEventData)
        {
            // 触发状态下（非奥义技能）：执行技能触发逻辑
            if (triggerPhase == E_TriggerPhase.Trigger && _SkillType != E_SkillType.UltimateSkill)
            {
                // 重置为选中状态（避免重复触发）
                triggerPhase = E_TriggerPhase.Selected;
                // 触发玩家技能执行事件（通知战斗系统释放技能）
                battleContext.GetEventBus().TriggerEvent(new RoleTriggerSkillEvent(battleContext, skillId, battleEntity));
            }
            else
            {
                // 终结技技能逻辑：释放终结技（临时直接调用，后续需优化）
                battleEntity.GetComponent<PlayerSkillComponent>().ReleaseUltimate();
            }
        }

        /// <summary>
        /// 重置技能按键状态
        /// 清理Toggle、状态机、缩放、绑定实体等数据
        /// </summary>
        private void ResetState()
        {
            // 解除Toggle分组绑定
            togSkillKeyUI.group = null;
            // 重置Toggle选中状态
            togSkillKeyUI.isOn = false;
            // 强制重置触发阶段为未选中
            triggerPhase = E_TriggerPhase.NonSeleceted;
            // 恢复默认缩放比例
            transform.localScale = Vector3.one;
            // 清空绑定的战斗实体
            battleEntity = null;
        }

        /// <summary>
        /// 组件禁用时的清理逻辑（OnDisable生命周期）
        /// 防止组件复用导致的状态残留
        /// </summary>
        protected override void OnDisable()
        {
            ResetState();
        }
    }
}