using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FireFly终结技弹射物
/// </summary>
public class FireFlyUltimateSkillProjectile : InstantProjectile
{
    protected override void OnInit()
    {
        dmgTimes = new float[] { 0.16f, 0.39f, 0.58f, 0.64f, 0.79f, 0.81f };
        // 可添加Buff
        base.OnInit();
    }
}
