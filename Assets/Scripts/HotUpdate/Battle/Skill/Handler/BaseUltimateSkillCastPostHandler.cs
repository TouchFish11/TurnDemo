using System.Collections;
using Core.Log;
using Core.Reflection;
using Core.Service;
using Core.UI;
using Core.Utility;
using HotUpdate.Battle.Input;
using HotUpdate.Battle.Object;
using HotUpdate.Battle.Skill.Base;
using HotUpdate.Battle.Skill.Component;
using HotUpdate.Battle.Skill.Enum;
using HotUpdate.Battle.TargetSelect;
using HotUpdate.Battle.TargetSelect.Strategys;
using HotUpdate.Battle.UI.Base;
using HotUpdate.Battle.UI.SkillKey;
using HotUpdate.Battle.UI.SkillKey.Provider;

namespace HotUpdate.Battle.Skill.Handler
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
        public IEnumerator Handle(ISkill skill)
        {
            // 获取技能释放者的战斗上下文
            var context = skill.Caster.Context;
            // 获取当前执行技能的实体（释放者）
            var currentEntity = context.GetCurrentEntity();

            // 判断能否处理
            if (!CanHandle(currentEntity))
            {
                yield break;
            }
            
            LogManager.Log($"角色：{skill.Caster}，终结技释放完毕，且可以执行后处理逻辑，当前行动角色：{currentEntity}");
            
            // 获取当前玩家的普通攻击技能信息（终极技能释放后，切回普攻的目标选择逻辑）
            var currentEntitySkillInfo = GetNormalSkillInfo(currentEntity);
            
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
            
            // 更新怪物位置
            context.GetProxy().UpdateMonsterPos(currentEntity);
            // 切换战斗相机至当前玩家实体视角
            yield return TaskUtility.WaitForTask(context.GetProxy().UpdateCamera(currentEntity));
            
            // 获取玩家基础目标选择策略
            var strategy = ServiceLocator.Get<IFactoryManager>().
                GetFactory<ITargetSelectStrategyFactory, TargetSelectStrategyFactory>()
                .GetTargetSelectStrategy<PlayerBaseTargetSelectStrategy>();
            
            // 激活目标选择
            ServiceLocator.Get<ITargetSelectManager>().ActiveSelectTarget();
            // 激活相机输入
            ServiceLocator.Get<IBattleInputHandler>().SetInputState(true);
            // 执行目标选择逻辑
            ServiceLocator.Get<ITargetSelectManager>().SelectTarget(context, currentEntity, currentEntitySkillInfo, strategy);
            // 重新激活怪物UI的血量显示
            ServiceLocator.Get<IUIManager>().GetController<BattleController>().MonsterStateUIManager.ActiveMonsterUIs();
        }

        private static SkillInfo GetNormalSkillInfo(IBattleEntityObject currentEntity)
        {
            var skillComponent = currentEntity.GetComponent<SkillComponent>();
            foreach (var s in skillComponent.GetSkills())
            {
                if (s.SkillInfo.f_SkillType == (byte)E_SkillType.NormalAttack)
                {
                    return s.SkillInfo;
                }
            }

            LogManager.LogError($"{nameof(BaseUltimateSkillCastPostHandler)}.{nameof(GetNormalSkillInfo)}：未找到普攻技能信息");
            return null;
        }

        private bool CanHandle(IBattleEntityObject currentEntity)
        {
            // 非玩家实体不执行后续逻辑（仅处理玩家释放终极技能的场景）;检查当前实体是否具备行动能力，无行动能力则终止流程
            return currentEntity is PlayerObject && currentEntity.CanAct;
        }
    }
}