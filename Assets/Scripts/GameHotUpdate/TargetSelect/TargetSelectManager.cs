using System.Collections.Generic;
using Core.DataPersistence.Binary;
using Core.Log;
using Core.Service;
using Core.Singleton;
using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Skill.Enum;
using Game.Battle.TargetSelect;
using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Event.UI;
using GameHotUpdate.Input;
using GameHotUpdate.Utility;

namespace GameHotUpdate.TargetSelect
{
    /// <summary>
    /// 目标选择管理器
    /// 核心职责：管理战斗中技能释放的目标选择逻辑，维护选中的主目标和范围目标列表，
    /// 响应技能选择、拖拽切换目标、点击选中目标等交互事件，同步更新目标选择UI
    /// 单例模式实现，全局唯一管理战斗目标选择流程
    /// </summary>
    public class TargetSelectManager : SingletonBase<TargetSelectManager>, ITargetSelectManager
    {
        // 已选中的范围目标列表（包含主目标及范围内的其他目标）
        private readonly List<IBattleEntityObject> _selectedTargets = new List<IBattleEntityObject>();
        // 当前选中的主目标（技能优先作用的核心目标）
        private IBattleEntityObject _mainTarget;
        // 当前选中技能的配置信息
        private SkillInfo skillInfo;
        // 战斗上下文
        private IBattleContext battleContext;
        // 技能释放者（释放当前技能的战斗实体）
        private IBattleEntityObject caster;
        // 当前生效的目标选择策略（不同技能有不同的目标选择规则）
        private ITargetSelectStrategy currentSelectStrategy;

        /// <summary>
        /// 私有构造函数
        /// 单例模式：禁止外部实例化，通过 SingletonBase 的 Instance 属性获取实例
        /// </summary>
        private TargetSelectManager()
        {

        }

        /// <summary>
        /// 初始化目标选择管理器
        /// </summary>
        /// <param name="battleContext">战斗上下文，提供战斗核心数据和事件总线</param>
        public void Init(IBattleContext battleContext)
        {
            this.battleContext = battleContext;
            // 注册技能选择事件监听：当玩家选择技能时触发目标选择逻辑
            battleContext.GetEventBus().AddListener<SelectSkillEvent>(OnSelectSkillEvent);
        }

        /// <summary>
        /// 激活目标选择交互
        /// 开启拖拽/点击切换目标的交互能力
        /// </summary>
        public void ActiveSelectTarget()
        {
            BattleInputHandler.Instance.OnLeftDrag += SelectPreviousMainTarget;   // 左拖拽：切换上一个主目标
            BattleInputHandler.Instance.OnRightDrag += SelectNextMainTarget;     // 右拖拽：切换下一个主目标
            BattleInputHandler.Instance.OnSelectedObject += SelectClickMainTarget;// 点击：选中指定主目标
        }

        /// <summary>
        /// 禁用目标选择交互
        /// 关闭拖拽/点击切换目标的交互能力，避免无效输入响应
        /// </summary>
        public void InActiveSelectTarget()
        {
            BattleInputHandler.Instance.OnLeftDrag -= SelectPreviousMainTarget;
            BattleInputHandler.Instance.OnRightDrag -= SelectNextMainTarget;
            BattleInputHandler.Instance.OnSelectedObject -= SelectClickMainTarget;
        }

        /// <summary>
        /// 核心目标选择入口
        /// 根据技能、释放者、选择策略重新计算并更新主目标和范围目标
        /// </summary>
        /// <param name="context">战斗上下文</param>
        /// <param name="caster">技能释放者</param>
        /// <param name="skillInfo">当前选中的技能配置</param>
        /// <param name="targetSelectStrategy">目标选择策略（决定如何选主目标）</param>
        public void SelectTarget(IBattleContext context, IBattleEntityObject caster, SkillInfo skillInfo, ITargetSelectStrategy targetSelectStrategy)
        {
            currentSelectStrategy = targetSelectStrategy;
            // 技能切换时，先重新选择主目标（触发UI更新）
            SelectMainTarget(context, caster, skillInfo);
            // 基于主目标更新范围目标列表
            UpdateTargets();
        }

        /// <summary>
        /// 获取当前选中的主目标
        /// </summary>
        /// <returns>主目标战斗实体</returns>
        public IBattleEntityObject GetMainTarget()
        {
            return _mainTarget;
        }

        /// <summary>
        /// 获取当前选中的所有范围目标（包含主目标）
        /// </summary>
        /// <returns>范围目标列表</returns>
        public List<IBattleEntityObject> GetTargets()
        {
            return _selectedTargets;
        }

        /// <summary>
        /// 技能选择事件回调
        /// 当玩家在UI中选中某个技能时触发，初始化目标选择的核心数据
        /// </summary>
        /// <param name="selectSkillEvent">技能选择事件（携带技能ID、释放者、战斗上下文等）</param>
        private void OnSelectSkillEvent(SelectSkillEvent selectSkillEvent)
        {
            // 从配置表加载选中技能的详细配置
            skillInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<SkillInfoContainer>(EConfigLoadType.Editor).dataDic[selectSkillEvent.SkillId];
            // 触发目标选择逻辑
            SelectTarget(selectSkillEvent.Context, selectSkillEvent.Caster, skillInfo, selectSkillEvent.TargetSelectStrategy);
        }

        /// <summary>
        /// 选择主目标
        /// 基于当前选择策略，计算并设置技能的核心作用目标
        /// </summary>
        /// <param name="context">战斗上下文</param>
        /// <param name="caster">技能释放者</param>
        /// <param name="skillInfo">技能配置（影响目标选择规则）</param>
        private void SelectMainTarget(IBattleContext context, IBattleEntityObject caster, SkillInfo skillInfo)
        {
            // 记录技能释放者，供后续范围目标计算使用
            this.caster = caster;
            // 委托给当前策略计算主目标
            _mainTarget = currentSelectStrategy.SelectMainTarget(context, caster, skillInfo);
            LogManager.Log($"当前主目标：{_mainTarget}");
        }

        /// <summary>
        /// 更新范围目标列表
        /// 基于主目标和技能范围规则，重新计算所有受影响的目标，并触发UI更新事件
        /// </summary>
        private void UpdateTargets()
        {
            // 清空旧的范围目标列表
            _selectedTargets.Clear();
            // 计算主目标范围内的所有有效目标（玩家角色类型，按技能范围规则筛选）
            BattleUtil.GetRangeTargets(E_CharacterType.PlayerCharacter, _mainTarget, skillInfo.f_skillRangeType, _selectedTargets);
            // 触发目标选择变更事件，通知UI更新选中状态
            battleContext.GetEventBus().TriggerEvent(new SelectTargetEvent(battleContext, _mainTarget, _selectedTargets));
        }

        /// <summary>
        /// 切换到下一个主目标
        /// 右拖拽交互触发，在同类型目标列表中向后切换主目标
        /// </summary>
        private void SelectNextMainTarget()
        {
            // 获取技能目标类型（友方/敌方），筛选对应类型的存活目标列表
            var targetType = (E_SkillTargetType)skillInfo.f_SkillTargetType;
            var targets = new List<IBattleEntityObject>();
            if (targetType == E_SkillTargetType.Friend)
            {
                battleContext.GetAlivePlayerEntitys(targets); // 友方：获取存活的玩家角色
            }
            else
            {
                battleContext.GetAliveMonsterEntitys(targets); // 敌方：获取存活的怪物角色
            }

            // 仅1个目标时无需切换
            if (targets.Count == 1)
                return;

            // 获取当前主目标在列表中的索引
            var mainIndex = targets.IndexOf(_mainTarget);
            // 索引未越界时，切换到下一个目标
            if (mainIndex + 1 < targets.Count)
            {
                _mainTarget = targets[++mainIndex];
                // 切换后更新范围目标列表并同步UI
                UpdateTargets();
            }
        }

        /// <summary>
        /// 切换到上一个主目标
        /// 左拖拽交互触发，在同类型目标列表中向前切换主目标
        /// </summary>
        private void SelectPreviousMainTarget()
        {
            // 获取技能目标类型（友方/敌方），筛选对应类型的存活目标列表
            var targetType = (E_SkillTargetType)skillInfo.f_SkillTargetType;
            var targets = new List<IBattleEntityObject>();
            if (targetType == E_SkillTargetType.Friend)
            {
                battleContext.GetAlivePlayerEntitys(targets); // 友方：获取存活的玩家角色
            }
            else
            {
                battleContext.GetAliveMonsterEntitys(targets); // 敌方：获取存活的怪物角色
            }

            // 仅1个目标时无需切换
            if (targets.Count == 1)
            {
                return;
            }

            // 获取当前主目标在列表中的索引
            var mainIndex = targets.IndexOf(_mainTarget);
            // 索引未越界时，切换到上一个目标
            if (mainIndex - 1 >= 0)
            {
                _mainTarget = targets[--mainIndex];
                // 切换后更新范围目标列表并同步UI
                UpdateTargets();
            }
        }

        /// <summary>
        /// 点击选中主目标
        /// 点击战斗实体时触发，直接将该实体设为主目标
        /// </summary>
        /// <param name="mainTarget">点击选中的战斗实体</param>
        private void SelectClickMainTarget(IBattleEntityObject mainTarget)
        {
            _mainTarget = mainTarget;
            // 选中后更新范围目标列表并同步UI
            UpdateTargets();
        }
    }
}