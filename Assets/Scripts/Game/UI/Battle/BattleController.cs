using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗界面控制器
/// </summary>
public class BattleController : UIController<BattleView, BattleModel>
{
    public BattleController(BattleView view, BattleModel model) : base(view, model)
    {

    }

    protected override void OnInit()
    {
        throw new NotImplementedException();
    }

    internal void BattleOver()
    {
        throw new NotImplementedException();
    }
}
