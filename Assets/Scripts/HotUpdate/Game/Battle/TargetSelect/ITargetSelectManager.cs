using System.Collections.Generic;

using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.TargetSelect
{
    /// <summary>
    /// 目标选择管理器接口
    /// </summary>
    public interface ITargetSelectManager
    {
        /// <summary>
        /// 获取主目标
        /// </summary>
        /// <returns></returns>
        IBattleEntityObject GetMainTarget();

        /// <summary>
        /// 获取所有目标
        /// 包含主目标
        /// </summary>
        /// <returns></returns>
        List<IBattleEntityObject> GetTargets();

        /// <summary>
        /// 根据技能和策略自动选择主目标
        /// </summary>
        /// <param name="context"></param>
        /// <param name="caster"></param>
        /// <param name="skillInfo"></param>
        /// <param name="targetSelectStrategy"></param>
        void SelectMainTarget(IBattleContext context, IBattleEntityObject caster, SkillInfo skillInfo, ITargetSelectStrategy targetSelectStrategy);
        
        /// <summary>
        /// 根据射线检测到的目标作为主目标
        /// </summary>
        /// <param name="mainTarget">点击选中的战斗实体</param>
        void SelectMainTarget(IBattleEntityObject mainTarget);
        
        /// <summary>
        /// 更新范围目标列表
        /// 基于主目标和技能范围规则，重新计算所有受影响的目标，并触发UI更新事件
        /// </summary>
        void SelectAllTargets(int skillRangeType);

        /// <summary>
        /// 切换到下一个主目标
        /// 右拖拽交互触发，在同类型目标列表中向后切换主目标
        /// </summary>
        void SelectNextMainTarget();

        /// <summary>
        /// 切换到上一个主目标
        /// 左拖拽交互触发，在同类型目标列表中向前切换主目标
        /// </summary>
        void SelectPreviousMainTarget();

        void Init(IBattleContext context);
        void Reset();
    }
}
