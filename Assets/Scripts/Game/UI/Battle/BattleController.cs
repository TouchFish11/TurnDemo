using Framework;
using Game;
using Game.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 战斗界面控制器工厂
/// </summary>
public class BattleControllerFactory : UIControllerFactory<BattleView, BattleModel, BattleController>
{
    public override BattleController CreateController(BattleView view, BattleModel model)
    {
        return new BattleController(view, model);
    }

    public override BattleModel CreateModel()
    {
        return new BattleModel();
    }
}

/// <summary>
/// 战斗界面控制器
/// </summary>
public class BattleController : UIController<BattleView, BattleModel>
{
    private GameObject actingFlagObj;
    public BattleController(BattleView view, BattleModel model) : base(view, model)
    {

    }

    /// <summary>
    /// 初始化战斗UI
    /// </summary>
    /// <param name="battleEntities"></param>
    public async Task InitBattleUI(IBattleContext battleContext)
    {
        await UpadteActionBar(battleContext.GetAllBattleEntity());
        await InitPlayerUI(battleContext.GetPlayerObjects());
        await InitMonsterUI(battleContext.GetMonsterObjects());

        battleContext.GetTurnManager().OnTurnStart += OnTurnStart;

    }

    private async Task InitPlayerUI(IEnumerable<IBattleEntityObject> battleEntities)
    {
        // 玩家角色显示UI
        foreach (IBattleEntityObject entityObject in battleEntities)
        {

        }
    }

    private async Task InitMonsterUI(IEnumerable<IBattleEntityObject> battleEntities)
    {
        // 怪物血量UI
        foreach (IBattleEntityObject entityObject in battleEntities)
        {

        }
    }

    /// <summary>
    /// 回合开始事件
    /// </summary>
    /// <param name="turnStartEvent"></param>
    private async void OnTurnStart(TurnStartEvent turnStartEvent)
    {
        // 更新行动轴显示
        await UpadteActionBar(turnStartEvent.Context.GetAllBattleEntity());
        // 更新当前操作UI
        UpdateOperator(turnStartEvent.CurrentBattleEntity);
        // 更新目标行动标识（Test）
        await UpdateActingFlag(turnStartEvent.CurrentBattleEntity.GameObject.transform.position);
    }

    /// <summary>
    /// 当前玩家操作UI
    /// </summary>
    /// <param name="currentObject"></param>
    private async void UpdateOperator(IBattleEntityObject currentObject)
    {
        if (currentObject is PlayerObject playerObject)
        {
            List<int> skills = new List<int>(currentObject.GetComponent<SkillComponent>().GetSkillIds());

            List<SkillKeyUI> skillKeyUIs = new List<SkillKeyUI>();
            // 遍历技能
            foreach (int skillId in skills)
            {
                // 当前玩家操作UI
                SkillKeyUI skillKeyUI = await ObjectBuilder.GetOrCreateInstance<SkillKeyUI>(E_AssetBundleType.UI, ResKeyCollection.SkillKeyUI, null);
                skillKeyUI.OnTriggerSkill += playerObject.GetComponent<SkillComponent>().CastSkill;
                SkillInfo skillInfo = BinaryDataMgr.Instance.GetConfig<SkillInfoContainer>(E_ConfigLoadType.Editor).dataDic[skillId];
                skillKeyUI.Init(skillInfo, playerObject.RoleInfo, _view.SkillKeyGroup);
                skillKeyUIs.Add(skillKeyUI);
            }

            _model.UpdateOperator(skillKeyUIs);
        }
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
    
    private async Task UpdateActingFlag(Vector3 worldPos)
    {
        if (actingFlagObj != null)
        {
            PoolManager.Instance.PushObj(actingFlagObj);
        }

        actingFlagObj = await ObjectBuilder.GetOrCreateInstance(E_AssetBundleType.UI, ResKeyCollection.ActingFlag, null);

        // HACK：UI坐标的偏移数值临时写死，后续根据需求调整
        UIManager.Instance.WorldToLocalPointInRectangle(Camera.main, UIManager.Instance.UICamera, _view.transform, actingFlagObj, worldPos, Vector2.up * 125f);
    }

    internal void BattleOver()
    {
        throw new NotImplementedException();
    }
}
