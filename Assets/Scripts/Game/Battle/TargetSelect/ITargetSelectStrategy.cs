using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 目标选择策略接口
/// </summary>
public interface ITargetSelectStrategy
{
    /// <summary>
    /// 选择主目标
    /// </summary>
    /// <param name="context"></param>
    /// <param name="caster"></param>
    /// <param name="skillInfo"></param>
    /// <returns></returns>
    IBattleEntityObject SelectMainTarget(IBattleContext context, IBattleEntityObject caster, SkillInfo skillInfo);
}
