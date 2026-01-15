using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Herta终结技技能弹射物
/// </summary>
public class HertaUltimateSkillProjectile : InstantProjectile
{
    protected override void OnInit()
    {
        dmgTimes = new float[] { 0.46f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f, };
        // 可添加Buff
        base.OnInit();
    }
}
