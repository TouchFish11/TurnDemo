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
    event Action<(IBattleEntityObject maintarget, List<IBattleEntityObject> selectedTargets)> OnTargetSelectionChanged;

    void ActiveSelectTarget(int skillId);

    void InActiveSelectTarget();

    void UpdateSkillSelect(int skillId);

    IBattleEntityObject GetMainTarget();

    List<IBattleEntityObject> GetTargets();

    void UpdateTargets();
}
