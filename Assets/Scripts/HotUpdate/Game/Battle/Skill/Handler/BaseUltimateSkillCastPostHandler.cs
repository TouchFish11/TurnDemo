using System.Collections;
using Core.DI;
using Core.Log;
using Core.Serialize.Binary;
using Core.Tasks;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Operation;
using HotUpdate.Game.Battle.Operation.Provider;
using HotUpdate.Game.Battle.Skill.Component;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.TargetSelect.Strategys;
using HotUpdate.Game.Battle.UI;

namespace HotUpdate.Game.Battle.Skill.Handler
{
    /// <summary>
    /// 终极技能释放后置处理器
    /// 作用：在玩家释放终极技能后，处理后续的UI更新、目标选择、相机切换等战斗流程相关逻辑
    /// </summary>
    public class BaseUltimateSkillCastPostHandler : SkillCastPostHandler
    {
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private IUIService _uiService;
        [Inject] private ISkillKeyUIDataProviderFactory _skillKeyUIDataProviderFactory;
        [Inject] private ITargetSelectStrategyFactory _targetSelectStrategyFactory;
        [Inject] private BattleCoordinator _battleCoordinator;
        [Inject] private IBattleCommandsController _battleCommandsController;

        protected override IEnumerator OnHandle()
        {
            // 获取当前回合的实体
            var currentEntity = BattleContext.CurrentTurnOwner;
            // 获取战斗控制器
            var battleController = (IBattleController)_uiService.GetPanel(EUIPanelId.BattlePanel);
            // 存在多个命令就显示下一个UI并并从等待列表中移除，这里只是处理UI显示，不会真正移除指令（由指令控制器处理）
            if (BattleContext.BattleCommands.Count > 0)
            {
                battleController.BattleUiManager.SetCurrentCommanderDisplayUI(BattleContext.BattleCommands[0].Sender);
            }
            // 否则没有命令就判断持有当前回合的角色能否行动
            else if(CanHandle(currentEntity))
            {
                // 获取当前玩家的普通攻击技能信息（终极技能释放后，切回普攻的目标选择逻辑）
                var currentEntitySkillInfo = GetNormalSkillInfo(currentEntity);
                // 隐藏战斗界面的行动提示UI（如技能释放提示、行动按钮等）
                battleController.BattleUiManager.SetActTipActive(EActTipType.Hide);
                // 获取技能按键UI数据提供者（用于更新玩家操作区的技能按键状态）
                var provider = _skillKeyUIDataProviderFactory.GetProvider<BaseSkillKeyUIDataProvider>();
                // 更新玩家操作界面（技能按键、可操作状态等）
                battleController.BattleUiManager.UpdateOperator(currentEntity, provider);
                // 显示当前回合角色UI并从等待列表中移除。
                battleController.BattleUiManager.RemoveFirstWaitingActUI();
                battleController.BattleUiManager.SetCurrentCommanderDisplayUI(currentEntity);
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
                battleController.MonsterStateUIManager.ActiveMonsterUIs();
            }
        }

        private SkillInfo GetNormalSkillInfo(IBattleEntityObject currentEntity)
        {
            var skillComponent = currentEntity.GetComponent<ISkillComponent>();
            foreach (var skillId in skillComponent.GetSkillIds())
            {
                var skillInfo = _binaryDataManager.GetConfig<SkillInfoContainer>(EConfigLoadType.Excel).dataDic[skillId];
                if (skillInfo.f_SkillType == (byte)E_SkillType.NormalAttack)
                {
                    return skillInfo;
                }
            }

            Logger.LogError(ELogTags.Battle, $"{nameof(BaseUltimateSkillCastPostHandler)}.{nameof(GetNormalSkillInfo)}：未找到普攻技能信息");
            return null;
        }

        private bool CanHandle(IBattleEntityObject currentEntity)
        {
            // 非玩家实体不执行后续逻辑（仅处理玩家释放终极技能的场景）;检查当前实体是否具备行动能力，无行动能力则终止流程
            return currentEntity is PlayerObject && currentEntity.CanAct;
        }
    }
}