using Core.DI;
using HotUpdate.Base;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.UI;

namespace HotUpdate.Game.Battle.Skill.Base
{
    /// <summary>
    /// 怪物技能抽象基类
    /// 所有怪物技能逻辑都需继承此类，封装怪物释放技能的通用前置逻辑
    /// </summary>
    public abstract class MonsterSkill : Skill
    {
        [Inject] private IUIService _uiService;
        
        /// <summary>
        /// 怪物技能构造函数
        /// </summary>
        /// <param name="caster">施法者（当前释放技能的怪物实体）</param>
        /// <param name="skillId">技能ID，用于标识不同技能</param>
        protected MonsterSkill(IBattleEntityObject caster, int skillId) : base(caster, skillId)
        {
            
        }

        /// <summary>
        /// 怪物技能释放前的预处理逻辑
        /// 包含目标选择、相机更新、UI重置、朝向同步等通用前置操作
        /// </summary>
        /// <param name="context">战斗上下文，包含当前战斗的所有环境信息和数据</param>
        protected sealed override void OnPreCast(IBattleContext context)
        {
            // 根据技能配置和选择策略，筛选出技能作用的目标
            //battleCoordinator.
            DIContainer.GetInstance<ITargetSelectManager>().SelectMainTarget(context, Caster, SkillInfo, TargetSelectStrategy);
            
            // 初始化技能目标数据，将选中的目标绑定到当前技能实例
            skillService.InitSkillTarget(this);
            
            // 关闭目标选择状态，避免技能释放过程中重复选目标
            battleCoordinator.IsActiveTargetSelect = false;
            
            // 获取战斗UI控制器，重置怪物相关UI（清空之前的选中/操作状态）
            var controller = _uiService.GetPanel(EUIPanelId.BattlePanel) as IBattleController;
            controller.MonsterStateUIManager.InActiveMonsterUIs();
            
            // 清除所有目标选中标记（UI层面隐藏选中框）
            controller.BattleUiManager.ClearSelectMarker();
            
            // 重置操作对象，取消当前选中的可操作实体
            controller.BattleUiManager.ClearOperator();
            
            // 激活战斗提示，显示怪物行动相关的提示类型（告知玩家当前是怪物回合/技能释放）
            controller.BattleUiManager.SetActTipActive(E_ActTipType.Monster);
            
            InitProjectile();
        }
        
        /// <summary>
        /// 初始化弹射物
        /// 子类技能重写，实现弹射物ProjectileData、ProjectileTrans、VFXInfo
        /// </summary>
        protected abstract void InitProjectile();
    }
}