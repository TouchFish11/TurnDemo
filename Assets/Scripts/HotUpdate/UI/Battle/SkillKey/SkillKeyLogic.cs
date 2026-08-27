using Core.DI;
using Core.Pool;
using Core.UI;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Component;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.TargetSelect.Strategys;
using HotUpdate.Game.Battle.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle.SkillKey
{
    public class SkillKeyLogic : IUILogic<SkillKeyUI, SkillKeyLogic>, IPoolData
    {
        [Inject] private IPoolManager _poolManager;
        [Inject] private ITargetSelectStrategyFactory _targetSelectStrategyFactory;
        
        /// <summary>
        /// 当前绑定的技能ID
        /// </summary>
        private int skillId;
        
        /// <summary>
        /// 选中时的缩放比例（视觉反馈）
        /// </summary>
        private readonly Vector3 SelectedScale = Vector3.one * 1.3f;
        
        /// <summary>
        /// 当前技能触发阶段（状态机核心变量）
        /// </summary>
        private ETriggerPhase triggerPhase = ETriggerPhase.NonSeleceted;
        
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

        
        public SkillKeyUI View { get; private set; }

        /// <summary>
        /// 初始化技能按键数据
        /// </summary>
        /// <param name="skillKeyUI"></param>
        /// <param name="skillInfo">技能配置信息</param>
        /// <param name="group">Toggle分组（用于互斥选中）</param>
        /// <param name="battleEntity">绑定的战斗实体（角色）</param>
        public void Init(SkillKeyUI skillKeyUI, SkillInfo skillInfo, ToggleGroup group, IBattleEntityObject battleEntity)
        {
            View = skillKeyUI;
            // 绑定技能ID
            skillId = skillInfo.f_id;
            // 设置Toggle分组（实现技能按键互斥选中）
            View.togSkillKeyUI.group = group;
            // 记录上下文
            battleContext = battleEntity.Context;
            // 绑定战斗实体
            this.battleEntity = battleEntity;
            // 从工厂获取玩家基础目标选择策略（目标选择的规则逻辑）
            _targetSelectStrategy = _targetSelectStrategyFactory.GetTargetSelectStrategy<PlayerBaseTargetSelectStrategy>();
            // 设置技能提示文本（显示技能类型）
            View.txtSkillTip.text = skillInfo.f_skillRangeType.ToSkillRangeTypeText();
            // 直接转换技能类型
            _SkillType = (E_SkillType)skillInfo.f_SkillType;
            // 普攻/终结技技能默认选中
            if (_SkillType is E_SkillType.NormalAttack or E_SkillType.UltimateSkill)
            {
                DefaultSelect();
                OnSelected(true);
            }
        }
        
        /// <summary>
        /// 处理技能选中状态变更
        /// </summary>
        /// <param name="isOn">是否选中</param>
        public void OnSelected(bool isOn)
        {
            if (isOn)
            {
                // 已选中→再次触发：切换为触发状态
                if (triggerPhase == ETriggerPhase.Selected)
                {
                    triggerPhase = ETriggerPhase.Trigger;
                }
                else
                {
                    // 未选中→选中：视觉缩放+状态更新+发送技能选中事件
                    View.transform.localScale = SelectedScale;
                    triggerPhase = ETriggerPhase.Selected;
                    // 触发技能选中事件
                    battleContext?.EventBus.TriggerEvent(new SelectSkillEvent(battleContext, skillId, battleEntity, _targetSelectStrategy));
                }
            }
            else
            {
                // 取消选中：恢复缩放+重置状态
                View.transform.localScale = Vector3.one;
                triggerPhase = ETriggerPhase.NonSeleceted;
            }
        }

        public void OnClick()
        {
            // 触发状态下（非终结技技能）：执行技能触发逻辑
            if (triggerPhase == ETriggerPhase.Trigger && _SkillType != E_SkillType.UltimateSkill)
            {
                // 重置为选中状态（避免重复触发）
                triggerPhase = ETriggerPhase.Selected;
                // 触发玩家技能执行事件（通知战斗系统释放技能）
                battleContext.EventBus.TriggerEvent(new RoleTriggerSkillEvent(battleContext, skillId, battleEntity));
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
            View.togSkillKeyUI.group = null;
            // 重置Toggle选中状态
            View.togSkillKeyUI.isOn = false;
            // 强制重置触发阶段为未选中
            triggerPhase = ETriggerPhase.NonSeleceted;
            // 恢复默认缩放比例
            View.transform.localScale = Vector3.one;
            // 清空绑定的战斗实体
            battleEntity = null;
        }
        
        /// <summary>
        /// 设置技能按键为默认选中状态
        /// </summary>
        public void DefaultSelect()
        {
            View.togSkillKeyUI.isOn = true;
        }
        
        public void ResetData()
        {
            ResetState();
        }
            
        public void Dispose()
        {
            _poolManager.PushData(this);
        }

    }
}
