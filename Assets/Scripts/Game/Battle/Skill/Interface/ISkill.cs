using System.Collections;
using System.Collections.Generic;

namespace Game.Battle
{
    /// <summary>
    /// 技能接口
    /// </summary>
    public interface ISkill
    {
        /// <summary>
        /// 技能配置
        /// </summary>
        SkillInfo SkillInfo { get; }

        /// <summary>
        /// 施法者
        /// </summary>
        IBattleEntityObject Caster { get; }

        /// <summary>
        /// 主目标
        /// </summary>
        IBattleEntityObject MainTarget { get; }

        /// <summary>
        /// 所有目标
        /// </summary>
        List<IBattleEntityObject> AllTargets { get; }

        /// <summary>
        /// 伤害计算管理器
        /// </summary>
        IDamageCalcManager DamageCalcManager { get; }

        /// <summary>
        /// 技能释放后处理器
        /// </summary>
        ISkillCastPostHandler SkillCastPostHandler { get; }

        /// <summary>
        /// 属性组件
        /// </summary>
        IPropertyComponent PropertyComponent { get; }

        /// <summary>
        /// 状态添加策略
        /// </summary>
        IStatusAddStrategy StatusAddStrategy { get; }

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="context"></param>
        /// <param name="caster"></param>
        /// <param name="targets"></param>
        IEnumerator Cast(IBattleContext context);

        /// <summary>
        /// 初始化技能
        /// </summary>
        /// <param name="mainTarget"></param>
        /// <param name="allTargets"></param>
        void Init(IBattleEntityObject mainTarget, List<IBattleEntityObject> allTargets);
    }
}
