using Game.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 目标选择管理器接口
/// </summary>
public interface ITargetSelectManager
{
    /// <summary>
    /// 激活目标选择
    /// </summary>
    void ActiveSelectTarget();

    /// <summary>
    /// 失活目标选择
    /// </summary>
    void InActiveSelectTarget();

    /// <summary>
    /// 获取主目标
    /// </summary>
    /// <returns></returns>
    IBattleEntityObject GetMainTarget();

    /// <summary>
    /// 获取目标列表（包含主目标）
    /// </summary>
    /// <returns></returns>
    List<IBattleEntityObject> GetTargets();

    /// <summary>
    /// 选择目标
    /// </summary>
    /// <param name="context"></param>
    /// <param name="caster"></param>
    /// <param name="skillInfo"></param>
    void SelectTarget(IBattleContext context, IBattleEntityObject caster, SkillInfo skillInfo, ITargetSelectStrategy targetSelectStrategy);
}
