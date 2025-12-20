using Framework;
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
    public async Task InitBattleUI(IEnumerable<IBattleEntityObject> battleEntities)
    {
        await UpadteActionBar(battleEntities);
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

    /// <summary>
    /// 更新行动条
    /// </summary>
    public async Task UpadteActionBar(IEnumerable<IBattleEntityObject> battleEntities)
    {
        List<ActionGridUI> actionGridUIs = new List<ActionGridUI>(); 
        foreach (IBattleEntityObject entityObject in battleEntities)
        {
            ActionGridUI actionGridUI = await ObjectBuilder.GetOrCreateInstance<ActionGridUI>(E_AssetBundleType.UI, ResKeyCollection.ActionGridUI, null);
            actionGridUI.Init(null, (int)entityObject.ActionValue);
            actionGridUIs.Add(actionGridUI);
        }
        _model.UpdateAcitonbar(actionGridUIs);
    }

    // 更新操作UI
    public void UpdateOperatorUI()
    {

    }


    internal void BattleOver()
    {
        throw new NotImplementedException();
    }
}
