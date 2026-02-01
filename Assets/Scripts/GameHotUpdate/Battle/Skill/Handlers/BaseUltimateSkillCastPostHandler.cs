using System.Collections;
using Core.Reflection;
using Core.Service;
using Core.UI;
using Game.Battle.Skill;
using Game.Battle.Skill.Component;
using Game.Battle.Skill.Enum;
using Game.Battle.Skill.Handler;
using Game.Battle.TargetSelect;
using Game.UI.Battle.SkillKey.Provider;
using GameHotUpdate.Battle.TargetSelect.Strategys;
using GameHotUpdate.Objects;
using GameHotUpdate.UI.Battle.Base;
using GameHotUpdate.UI.Battle.SkillKey.Provider;

namespace GameHotUpdate.Battle.Skill.Handlers
{
    /// <summary>
    /// 终极技能释放后置处理器
    /// 作用：在玩家释放终极技能后，处理后续的UI更新、目标选择、相机切换等战斗流程相关逻辑
    /// </summary>
    public class BaseUltimateSkillCastPostHandler : ISkillCastPostHandler
    {
        /// <summary>
        /// 处理终极技能释放后的后续逻辑
        /// </summary>
        /// <param name="skill">当前释放的技能实例</param>
        /// <returns>协程迭代器</returns>
        public IEnumerator OnHandle(ISkill skill)
        {
            // 获取技能释放者的战斗上下文
            var context = skill.Caster.Context;
            // 获取当前执行技能的实体（释放者）
            var currentEntity = context.GetCurrentEntity();

            // 战斗已结束则直接终止流程
            if (context.GetTurnManager().IsBattleOver)
            {
                yield break;
            }

            // 非玩家实体不执行后续逻辑（仅处理玩家释放终极技能的场景）
            if (currentEntity is not PlayerObject)
            {
                yield break;
            }
            
            // 检查当前实体是否具备行动能力（如未被眩晕、冰冻等控制），无行动能力则终止流程
            if (!currentEntity.CanAct)
            {
                yield break;
            }
            
            // 获取当前玩家的普通攻击技能信息（终极技能释放后，切回普攻的目标选择逻辑）
            var skillComponent = currentEntity.GetComponent<SkillComponent>();
            SkillInfo currentEntitySkillInfo = null;
            foreach (var s in skillComponent.GetSkills())
            {
                if (s.SkillInfo.f_SkillType == (byte)E_SkillType.NormalAttack)
                {
                    currentEntitySkillInfo = s.SkillInfo;
                }
            }
            
            // 隐藏战斗界面的行动提示UI（如技能释放提示、行动按钮等）
            ServiceLocator.Get<IUIManager>()
                .GetController<BattleController>()
                .BattleUiManager
                .SetActTipActive(E_ActTipType.Hide);
            
            // 获取技能按键UI数据提供者（用于更新玩家操作区的技能按键状态）
            var provider = ServiceLocator.Get<IFactoryManager>().
                GetFactory<ISkillKeyUIDataProviderFactory, SkillKeyUIDataProviderFactory>().
                GetCastSkillCondition<BaseSkillKeyUIDataProvider>();
            // 更新玩家操作界面（技能按键、可操作状态等）
            ServiceLocator.Get<IUIManager>()
                .GetController<BattleController>()
                .BattleUiManager
                .UpdateOperator(currentEntity, provider);
            
            // 切换战斗相机至当前玩家实体视角（聚焦释放技能的玩家）
            BattlePoint.Instance.ActiveCamera(currentEntity);
            
            // 更新实体朝向（让当前实体面向目标方向/默认方向）
            context.GetTurnManager().UpdateEntityLookAt(currentEntity);
            
            // 获取玩家基础目标选择策略（用于普攻的目标筛选逻辑）
            var strategy = ServiceLocator.Get<IFactoryManager>().
                GetFactory<ITargetSelectStrategyFactory, TargetSelectStrategyFactory>()
                .GetTargetSelectStrategy<PlayerBaseTargetSelectStrategy>();
            // 执行目标选择逻辑（为后续普攻选择可攻击的目标）
            ServiceLocator.Get<ITargetSelectManager>()
                .SelectTarget(context, currentEntity, currentEntitySkillInfo, strategy);
            
            // 重新初始化怪物UI的血量显示（同步当前存活怪物的血量、状态等信息）
            ServiceLocator.Get<IUIManager>()
                .GetController<BattleController>()
                .UiInitializer
                .InitMonsterUI(context.GetAliveMonsterEntitys());
        }
    }
}