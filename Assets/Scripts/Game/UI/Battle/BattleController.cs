using Game.Battle;
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

    /// <summary>
    /// 初始化战斗UI
    /// </summary>
    /// <param name="battleEntities"></param>
    public async Task InitBattleUI(List<IBattleEntityObject> battleEntities)
    {
        await InitPlayerUI();
        await InitMonsterUI();
    }

    private async Task InitPlayerUI()
    {
        // 玩家操作UI、玩家角色显示UI、行动条UI等
    }

    private async Task InitMonsterUI()
    {
        // 怪物血量UI
    }


    internal void BattleOver()
    {
        throw new NotImplementedException();
    }
}
