using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Herta角色普攻技能弹射物
/// </summary>
public class HertaNormalSKillProjectile : InstantProjectile
{
    protected override void OnInit()
    {
        dmgTimes = new float[] { 0.29f };  //TODO：可配置
        // 可添加Buff
        base.OnInit();
    }
}
