using Game.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockTargetSelect : ITargetSelect
{
    public event Action<(IBattleEntityObject maintarget, List<IBattleEntityObject> selectedTargets)> OnTargetSelectionChanged;

    public void ActiveSelectTarget(int skillId)
    {

    }

    public IBattleEntityObject GetMainTarget()
    {
        return GameObject.FindFirstObjectByType<MonsterObject>();
    }

    public List<IBattleEntityObject> GetTargets()
    {
        return new List<IBattleEntityObject>(GameObject.FindObjectsByType<MonsterObject>(FindObjectsSortMode.None));
    }

    public void InActiveSelectTarget()
    {

    }

    public void UpdateSkillSelect(int skillId)
    {

    }

    public void UpdateTargets(IBattleEntityObject mainTarget, List<IBattleEntityObject> targets)
    {

    }

    public void UpdateTargets()
    {

    }
}
