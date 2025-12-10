using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 战斗界面控制器
/// </summary>
public class BattleController : UIController<BattleView, BattleModel>
{
    public BattleController(BattleView view, BattleModel model) : base(view, model)
    {

    }

    protected override async Task OnInit()
    {

    }

    internal void BattleOver()
    {
        throw new NotImplementedException();
    }
}
