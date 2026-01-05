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
        /// 伤害计算管理器接口
        /// </summary>
        IDamageCalcManager DamageCalcManager { get; }
        ISkillCastPostHandler SkillCastPostHandler { get; }
        IPropertyComponent PropertyComponent { get; }

        /// <summary>
        /// 初始化技能
        /// </summary>
        /// <param name="skillId"></param>
        void Init(IBattleEntityObject caster, IBattleEntityObject mainTarget, List<IBattleEntityObject> allTargets);

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="context"></param>
        /// <param name="caster"></param>
        /// <param name="targets"></param>
        IEnumerator Cast(IBattleContext context); 
    }
}
