using Game.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 目标选择接口
/// </summary>
public interface ITargetSelect
{
    void ActiveSelectTarget(int skillId);

    void InActiveSelectTarget();

    void UpdateSkillSelect(int skillId);

    IBattleEntityObject GetMainTarget();

    List<IBattleEntityObject> GetTargets();

    void UpdateTargets();
}
