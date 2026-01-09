using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ╠сссп╖╧Ш
/// </summary>
public class ProtectStatus : Status
{
    protected override void OnAdd()
    {

    }

    protected override void OnPineChanged()
    {
        bonusData.DefBuildBonus += 20;
    }

    protected override void OnRemove()
    {

    }
}
