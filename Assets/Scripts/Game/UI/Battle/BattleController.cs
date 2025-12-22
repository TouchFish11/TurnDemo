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
        UpdateBattlePoint();
        await UpadteActionBar(battleContext.GetAllBattleEntity());
        await InitPlayerUI(battleContext.GetPlayerObjects());
        await InitMonsterUI(battleContext.GetMonsterObjects());

        battleContext.GetTurnManager().OnTurnStart += OnTurnStart;
        model.UpdateBattlePointCount(battleContext.BattlePointCount);
    }

    /// <summary>
    /// 初始化玩家UI
    /// </summary>
    /// <param name="battleEntities"></param>
    /// <returns></returns>
    private async Task InitPlayerUI(IEnumerable<IBattleEntityObject> battleEntities)
    {
        List<RoleStateUI> roleStateUIs = new List<RoleStateUI>();
        // 玩家角色显示UI
        foreach (IBattleEntityObject entityObject in battleEntities)
        {
            RoleStateUI roleStateUI = await ObjectBuilder.GetOrCreateInstance<RoleStateUI>(E_AssetBundleType.UI, ResKeyCollection.RoleStateUI, null);
            int skillId = entityObject.GetComponent<SkillComponent>().GetUltimateSkill();
            if (skillId != -1)
            {
                // TODO：暂时用对象级事件，后续优化为局部事件中心
                roleStateUI.OnTriggerUltimateSkill += entityObject.CastSkill;
                roleStateUI.Init((entityObject as PlayerObject).RoleInfo, skillId);
                roleStateUIs.Add(roleStateUI);
            }
        }

        model.InitRoleStateUI(roleStateUIs);
    }

    /// <summary>
    /// 初始化怪物UI
    /// </summary>
    /// <param name="battleEntities"></param>
    /// <returns></returns>
    private async Task InitMonsterUI(IEnumerable<IBattleEntityObject> battleEntities)
    {
        // 怪物血量UI
        foreach (IBattleEntityObject entityObject in battleEntities)
        {

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
        model.UpdateAcitonbar(actionGridUIs);
    }

    /// <summary>
    /// 更新当前玩家操作UI
    /// </summary>
    /// <param name="currentObject"></param>
    private async void UpdateOperator(IBattleEntityObject currentObject)
    {
        if (currentObject is PlayerObject playerObject)
        {
            List<ISkill> skills = new List<ISkill>(currentObject.GetComponent<SkillComponent>().GetSkills());

            List<SkillKeyUI> skillKeyUIs = new List<SkillKeyUI>();
            // 遍历技能
            foreach (ISkill skill in skills)
            {
                if (skill.SkillInfo.f_SkillType.ToSkillType() == E_SkillType.UltimateSkill)
                {
                    continue;
                }

                // 当前玩家操作UI
                SkillKeyUI skillKeyUI = await ObjectBuilder.GetOrCreateInstance<SkillKeyUI>(E_AssetBundleType.UI, ResKeyCollection.SkillKeyUI, null);
                skillKeyUI.OnTriggerSkill += playerObject.CastSkill;
                skillKeyUI.Init(skill.SkillInfo, playerObject.RoleInfo, view.SkillKeyGroup);
                skillKeyUIs.Add(skillKeyUI);
            }

            model.UpdateOperator(skillKeyUIs);
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
    /// 更新战技点数
    /// </summary>
    private void UpdateBattlePoint()
    {

    }

    /// <summary>
    /// 更新目标行动标识（Test）
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    private async Task UpdateActingFlag(Vector3 worldPos)
    {
        if (actingFlagObj != null)
        {
            PoolManager.Instance.PushObj(actingFlagObj);
        }

        actingFlagObj = await ObjectBuilder.GetOrCreateInstance(E_AssetBundleType.UI, ResKeyCollection.ActingFlag, null);

        // HACK：UI坐标的偏移数值临时写死，后续根据需求调整
        UIManager.Instance.WorldToLocalPointInRectangle(Camera.main, UIManager.Instance.UICamera, view.transform, actingFlagObj, worldPos, Vector2.up * 125f);
    }

    internal void BattleOver()
    {
        throw new NotImplementedException();
    }
}
