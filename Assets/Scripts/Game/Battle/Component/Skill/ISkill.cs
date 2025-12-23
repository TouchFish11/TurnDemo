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

        IBattleEntityObject Caster { get; }

        IBattleEntityObject MainTarget { get; }

        List<IBattleEntityObject> AllTargets { get; }

        /// <summary>
        /// 伤害系数（配置表读取）
        /// </summary>
        float DamageCoefficient { get; }

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
