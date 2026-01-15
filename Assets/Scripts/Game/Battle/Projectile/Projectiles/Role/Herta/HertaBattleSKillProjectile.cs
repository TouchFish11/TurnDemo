using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Herta战技技能弹射物
/// </summary>
public class HertaBattleSKillProjectile : InstantProjectile
{
    protected override void OnInit()
    {
        dmgTimes = new float[] { 1.52f };
        // 可添加Buff
        base.OnInit();
    }
}
