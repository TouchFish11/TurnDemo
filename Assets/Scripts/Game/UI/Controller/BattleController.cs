using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : UIController<BattleView, BattleModel>
{
    public BattleController(BattleView view, BattleModel model) : base(view, model)
    {

    }

    internal void BattleOver()
    {
        throw new NotImplementedException();
    }
}
