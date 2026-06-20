using System.Collections;
using Core.DI;
using Core.Log;
using Core.Serialize.Binary;
using Core.Utility;
using HotUpdate.Base.UI;
using HotUpdate.Common.Config.ExcelInfo.Container;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Component;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.TargetSelect.Strategys;
using HotUpdate.Game.Battle.UI;
using HotUpdate.Game.Battle.UI.Provider;

namespace HotUpdate.Game.Battle.Skill.Handler
{
    /// <summary>
    /// 终极技能释放后置处理器
    /// 作用：在玩家释放终极技能后，处理后续的UI更新、目标选择、相机切换等战斗流程相关逻辑
    /// </summary>
    public class BaseUltimateSkillCastPostHandler : ISkillCastPostHandler
    {
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private IUIService _uiService;
        [Inject] private ISkillKeyUIDataProviderFactory _skillKeyUIDataProviderFactory;
        [Inject] private ITargetSelectStrategyFactory _targetSelectStrategyFactory;
        [Inject] private BattleCoordinator _battleCoordinator;

        /// <summary>
        /// 处理终极技能释放后的后续逻辑
        /// </summary>
        /// <param name="skillContext"></param>
        /// <returns>协程迭代器</returns>
        public IEnumerator Handle(SkillContext skillContext)
        {
            // 获取技能释放者的战斗上下文
            var context = skillContext.Caster.Context;
            // 获取当前执行技能的实体（释放者）
            var currentEntity = context.GetCurrentEntity();

            // 判断能否处理
            if (!CanHandle(currentEntity))
            {
                yield break;
            }
            
            Logger.Log($"角色：{skillContext.Caster}，终结技释放完毕，且可以执行后处理逻辑，当前行动角色：{currentEntity}");
            
            // 获取当前玩家的普通攻击技能信息（终极技能释放后，切回普攻的目标选择逻辑）
            var currentEntitySkillInfo = GetNormalSkillInfo(currentEntity);
            
            // 隐藏战斗界面的行动提示UI（如技能释放提示、行动按钮等）
            ((IBattleController)_uiService.GetPanel(EUIPanelId.BattlePanel)).BattleUiManager.SetActTipActive(E_ActTipType.Hide);
            
            // 获取技能按键UI数据提供者（用于更新玩家操作区的技能按键状态）
            var provider = _skillKeyUIDataProviderFactory.GetCastSkillCondition<BaseSkillKeyUIDataProvider>();
            // 更新玩家操作界面（技能按键、可操作状态等）
            ((IBattleController)_uiService.GetPanel(EUIPanelId.BattlePanel)).BattleUiManager.UpdateOperator(currentEntity, provider);
            
            // 更新怪物位置
            _battleCoordinator.UpdateMonsterPos(currentEntity);
            // 切换战斗相机至当前玩家实体视角
            yield return TaskUtility.WaitForTask(_battleCoordinator.UpdateCamera((PlayerObject)currentEntity));
            
            // 获取玩家基础目标选择策略
            var strategy = _targetSelectStrategyFactory.GetTargetSelectStrategy<PlayerBaseTargetSelectStrategy>();
            
            // 激活目标选择
            _battleCoordinator.IsActiveTargetSelect = true;
            // 激活相机输入
            _battleCoordinator.IsActiveInput = true;
            // 执行目标选择逻辑
            _battleCoordinator.SetSelectSkillInfo(currentEntitySkillInfo);
            _battleCoordinator.SelectTargets(currentEntity, strategy);
            // 重新激活怪物UI的血量显示
            ((IBattleController)_uiService.GetPanel(EUIPanelId.BattlePanel)).MonsterStateUIManager.ActiveMonsterUIs();
        }

        private SkillInfo GetNormalSkillInfo(IBattleEntityObject currentEntity)
        {
            var skillComponent = currentEntity.GetComponent<SkillComponent>();
            foreach (var skillId in skillComponent.GetSkillIds())
            {
                var skillInfo = _binaryDataManager.GetConfig<SkillInfoContainer>(EConfigLoadType.Excel).dataDic[skillId];
                if (skillInfo.f_SkillType == (byte)E_SkillType.NormalAttack)
                {
                    return skillInfo;
                }
            }

            Logger.LogError($"{nameof(BaseUltimateSkillCastPostHandler)}.{nameof(GetNormalSkillInfo)}：未找到普攻技能信息");
            return null;
        }

        private static bool CanHandle(IBattleEntityObject currentEntity)
        {
            // 非玩家实体不执行后续逻辑（仅处理玩家释放终极技能的场景）;检查当前实体是否具备行动能力，无行动能力则终止流程
            return currentEntity is PlayerObject && currentEntity.CanAct;
        }
    }
}