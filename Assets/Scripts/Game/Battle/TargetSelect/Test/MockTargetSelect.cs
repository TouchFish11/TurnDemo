using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockTargetSelect : ITargetSelect
{
    public void ActiveSelectTarget(SkillInfo skillInfo)
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

    public void UpdateTargets(IBattleEntityObject mainTarget, List<IBattleEntityObject> targets)
    {

    }

    public void UpdateTargets()
    {

    }
}
