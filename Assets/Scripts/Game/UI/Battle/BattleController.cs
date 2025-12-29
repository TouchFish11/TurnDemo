using Framework;
using Game;
using Game.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
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
    private Vector2 damageTextXOffsetRange = new Vector2(-40, 40);
    private Vector2 damageTextYOffsetRange = new Vector2(-10, 10);

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
        await UpdateBattlePointCount(battleContext.CurentBattlePointCount, battleContext.MaxBattlePointCount);

        battleContext.GetEventBus().AddListener<OnBattlePointCountChangedEvent>(OnBattlePointCountChanged);
        battleContext.GetEventBus().AddListener<TurnStartEvent>(OnTurnStart);
        battleContext.GetEventBus().AddListener<TurnEndEvent>(OnTurnEnd);
        //battleContext.GetEventBus().AddListener<OnHpChangedEvent>(OnHpChanged);
        battleContext.GetEventBus().AddListener<OnTakeDamageEvent>(OnTakeDamage);

        battleContext.GetEventBus().AddListener<TriggerSkillEvent>(UpdateActTip);

        TargetSelectManager.Instance.RegisterTargetSelectionChanged(OnTargetSelectionChanged);
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
        foreach (IBattleEntityObject battleEntity in battleEntities)
        {
            RoleStateUI roleStateUI = await ObjectBuilder.GetOrCreateInstance<RoleStateUI>(E_AssetBundleType.UI, ResKeyCollection.RoleStateUI, null);
            int skillId = battleEntity.GetComponent<SkillComponent>().GetUltimateSkill();
            if (skillId != -1)
            {
                roleStateUI.Init(battleEntity.GetComponent<PlayerPropertyComponent>().GetProperty<RoleProperty>(), skillId, battleEntity);
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
    /// 更新行动提示UI
    /// </summary>
    private void UpdateActTip(TriggerSkillEvent triggerSkillEvent)
    {
        // 隐藏玩家操作UI
        model.UpdateOperator(new List<SkillKeyUI>());

        // 显示行动提示UI
        model.SetActTipActive(true, triggerSkillEvent.BattleEntity is MonsterObject);
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
            // 隐藏行动提示
            model.SetActTipActive(false, false);

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
                skillKeyUI.Init(skill.SkillInfo, playerObject.RoleInfo, view.SkillKeyGroup, currentObject);
                skillKeyUIs.Add(skillKeyUI);
            }

            model.UpdateOperator(skillKeyUIs);
        }
    }

    /// <summary>
    /// 目标选择变化事件回调
    /// </summary>
    /// <param name="info"></param>
    private async void OnTargetSelectionChanged((IBattleEntityObject maintarget, List<IBattleEntityObject> selectedTargets) info)
    {
        List<SelectMarkerUI> selectMarkerUIs = new List<SelectMarkerUI>();
        // 更新目标标记UI显示
        foreach (IBattleEntityObject battleEntity in info.selectedTargets)
        {
            SelectMarkerUI selectMarkerUI = await ObjectBuilder.GetOrCreateInstance<SelectMarkerUI>(E_AssetBundleType.UI, ResKeyCollection.SelectMarkerUI, null);
            if (UIManager.WorldToLocalPointInRectangle(BattlePoint.Instance.CurrentActiveCamera, UIManager.Instance.UICamera, view.SelectMarkerArea, selectMarkerUI.gameObject, battleEntity.GameObject.transform.position, Vector2.up * 50))
            {
                selectMarkerUI.InitSelectMarker((battleEntity is PlayerObject) ? E_SkillTargetType.Friend : E_SkillTargetType.Enemy);
                selectMarkerUIs.Add(selectMarkerUI);
            }
        }
        model.UpdateSelectMarker(selectMarkerUIs);

        // 更新行动轴目标高亮显示
    }

    /// <summary>
    /// 回合开始事件监听
    /// </summary>
    /// <param name="turnStartEvent"></param>
    private async void OnTurnStart(TurnStartEvent turnStartEvent)
    {
        // 更新行动轴显示
        await UpadteActionBar(turnStartEvent.Context.GetAllBattleEntity());
        // 更新当前操作UI
        UpdateOperator(turnStartEvent.CurrentBattleEntity);
        // 更新目标行动标识（Test）
        //await UpdateActingFlag(turnStartEvent.CurrentBattleEntity.GameObject.transform.position);
    }

    /// <summary>
    /// 回合结束事件监听
    /// </summary>
    /// <param name="turnEndEvent"></param>
    private void OnTurnEnd(TurnEndEvent turnEndEvent)
    {
        // TODO：不是回合结束，而是造成伤害的指令结束后清空
        model.UpdateCumulativeDamage(false, 0);
        TargetSelectManager.Instance.InActiveSelectTarget();
        // 清除UI
        model.ClearSelectMarker();
    }

    /// <summary>
    /// 受到伤害回调事件
    /// </summary>
    /// <param name="onTakeDamageEvent"></param>
    private async void OnTakeDamage(OnTakeDamageEvent onTakeDamageEvent)
    {
        DamageResult damageResult = onTakeDamageEvent.DamageResult;
        if (damageResult.Target is not MonsterObject)
        {
            return;
        }

        DamageTextUI damageTextUI = await ObjectBuilder.GetOrCreateInstance<DamageTextUI>(E_AssetBundleType.UI, ResKeyCollection.DamageTextUI, null);
        Vector2 dmgTextOffset = new Vector2(UnityEngine.Random.Range(damageTextXOffsetRange.x, damageTextXOffsetRange.y), UnityEngine.Random.Range(damageTextYOffsetRange.x, damageTextYOffsetRange.y));
        //坐标转换，初始化
        if (UIManager.WorldToLocalPointInRectangle(BattlePoint.Instance.CurrentActiveCamera, UIManager.Instance.UICamera, view.transform, damageTextUI.gameObject, damageResult.Target.GameObject.transform.position, Vector2.up * 50 + dmgTextOffset))
        {
            string critText = damageResult.IsCrit ? "暴击" : "";
            damageTextUI.InitDamageText(((int)damageResult.ElementType).ToElementTypeColor(), critText, damageResult.FinalDamage);
        }

        // 更新累计伤害
        model.UpdateCumulativeDamage(true, damageResult.FinalDamage);
    }

    /// <summary>
    /// 血量变化回调事件
    /// </summary>
    /// <param name="onHpChangedEvent"></param>
    private async void OnHpChanged(OnHpChangedEvent onHpChangedEvent)
    {
        // 显示伤害/治疗文本
        //DamageTextUI damageTextUI = await ObjectBuilder.GetOrCreateInstance<DamageTextUI>(E_AssetBundleType.UI, ResKeyCollection.DamageTextUI, null);
        //damageTextUI.InitDamageText();
    }

    /// <summary>
    /// 战技点变化事件
    /// </summary>
    /// <param name="battlePointCountChanged"></param>
    private async void OnBattlePointCountChanged(OnBattlePointCountChangedEvent battlePointCountChanged)
    {
        await UpdateBattlePointCount(battlePointCountChanged.CurentBattlePointCount, battlePointCountChanged.MaxBattlePointCount);
    }

    /// <summary>
    /// 更新战技点数
    /// </summary>
    /// <param name="current"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private async Task UpdateBattlePointCount(int current, int max)
    {
        List<BattlePointUI> battlePointUIs = new List<BattlePointUI>();
        for (int i = 0; i < max; i++)
        {
            BattlePointUI battlePointUI = await ObjectBuilder.GetOrCreateInstance<BattlePointUI>(E_AssetBundleType.UI, ResKeyCollection.BattlePointUI, null);
            battlePointUI.SetActivePoint(i < current);
            battlePointUIs.Add(battlePointUI);
        }

        model.UpdateBattlePointCount(current, battlePointUIs);
    }

    ///// <summary>
    ///// 更新目标行动标识（Test）
    ///// </summary>
    ///// <param name="worldPos"></param>
    ///// <returns></returns>
    //private async Task UpdateActingFlag(Vector3 worldPos)
    //{
    //    if (actingFlagObj != null)
    //    {
    //        PoolManager.Instance.PushObj(actingFlagObj);
    //    }

    //    actingFlagObj = await ObjectBuilder.GetOrCreateInstance(E_AssetBundleType.UI, ResKeyCollection.ActingFlag, null);

    //    // HACK：UI坐标的偏移数值临时写死，后续根据需求调整
    //    UIManager.WorldToLocalPointInRectangle(Camera.main, UIManager.Instance.UICamera, view.transform, actingFlagObj, worldPos, Vector2.up * 125f);
    //}

    public override void Destroy()
    {
        TargetSelectManager.Instance.CancelTargetSelectionChanged(OnTargetSelectionChanged);
        base.Destroy();
    }
    internal void BattleOver()
    {
        throw new NotImplementedException();
    }
}
