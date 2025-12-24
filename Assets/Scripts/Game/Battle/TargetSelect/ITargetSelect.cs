using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 目标选择接口
/// </summary>
public interface ITargetSelect
{
    void ActiveSelectTarget(SkillInfo skillInfo);

    void InActiveSelectTarget();

    IBattleEntityObject GetMainTarget();

    List<IBattleEntityObject> GetTargets();

    void UpdateTargets();
}
