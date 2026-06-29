using System.Collections;
using Core.DI;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Operation;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.UI;

namespace HotUpdate.Game.Battle.Skill.Nodes
{
    /// <summary>
    /// 怪物效果执行前的统一前置效果
    /// </summary>
    public class MonsterPreNode : SkillNode
    {
        [Inject] private IUIService uiService;
        
        public MonsterPreNode(ISkill skill) : base(skill)
        {
            
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override IEnumerator Execute()
        {
            // 关闭目标选择状态，避免技能释放过程中重复选目标
            battleCoordinator.IsActiveTargetSelect = false;
            // 获取战斗UI控制器，重置怪物相关UI（清空之前的选中/操作状态）
            var controller = (IBattleController)uiService.GetPanel(EUIPanelId.BattlePanel);
            controller.MonsterStateUIManager.InActiveMonsterUIs();
            // 清除所有目标选中标记（UI层面隐藏选中框）
            controller.BattleUiManager.ClearSelectMarker();
            // 重置操作对象，取消当前选中的可操作实体
            controller.BattleUiManager.ClearOperator();
            // 激活战斗提示，显示怪物行动相关的提示类型（告知玩家当前是怪物回合/技能释放）
            controller.BattleUiManager.SetActTipActive(EActTipType.Monster);
            yield break;
        }
    }
}
