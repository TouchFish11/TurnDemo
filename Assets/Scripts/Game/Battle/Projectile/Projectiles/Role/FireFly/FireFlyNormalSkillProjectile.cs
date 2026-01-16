using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FireFly∆’π•ººƒ‹µØ…‰ŒÔ
/// </summary>
public class FireFlyNormalSkillProjectile : InstantProjectile
{
    protected override void OnInit()
    {
        dmgTimes = new float[] { 0.2f };

        base.OnInit();
    }
}
