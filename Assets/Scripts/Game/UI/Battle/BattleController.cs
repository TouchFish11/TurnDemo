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
        await InitPlayerUI(battleContext.GetPlayerObjects());
        await UpdateBattlePointCount(battleContext.CurentBattlePointCount, battleContext.MaxBattlePointCount);

        battleContext.GetEventBus().AddListener<OnBattlePointCountChangedEvent>(OnBattlePointCountChanged);
        battleContext.GetEventBus().AddListener<TurnStartEvent>(OnTurnStart);
        battleContext.GetEventBus().AddListener<TurnEndEvent>(OnTurnEnd);

        //battleContext.GetEventBus().AddListener<HpChangedEvent>(OnHpChanged);
        battleContext.GetEventBus().AddListener<TakeDamageEvent>(OnTakeDamage);
        battleContext.GetEventBus().AddListener<PlayerTriggerSkillEvent>(UpdateActTip);
        battleContext.GetEventBus().AddListener<SelectTargetEvent>(OnTargetSelectionChanged);
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
    /// 依赖玩家相机初始化完毕
    /// </summary>
    /// <param name="battleEntities"></param>
    /// <returns></returns>
    private async Task InitMonsterUI(IEnumerable<IBattleEntityObject> battleEntities)
    {
        List<NormalMonsterStateUI> normalMonsterStateUIs = new List<NormalMonsterStateUI>(); 
        // 怪物血量UI
        foreach (IBattleEntityObject battleEntity in battleEntities)
        {
            NormalMonsterStateUI monsterStateUI = await ObjectBuilder.GetOrCreateInstance<NormalMonsterStateUI>(E_AssetBundleType.UI, ResKeyCollection.MonsterStateUI, null);
            if (UIManager.WorldToLocalPointInRectangle(BattlePoint.Instance.CurrentActiveCamera, UIManager.Instance.UICamera, view.MonsterStateArea, monsterStateUI.gameObject, battleEntity.GameObject.transform.position, Vector2.up * 250))
            {
                monsterStateUI.Init(battleEntity);
                normalMonsterStateUIs.Add(monsterStateUI);
            }
        }

        model.UpdateNormalMonsterState(normalMonsterStateUIs);
    }

    /// <summary>
    /// 更新行动提示UI
    /// </summary>
    private void UpdateActTip(PlayerTriggerSkillEvent playerTriggerSkillEvent)
    {
        // 隐藏玩家操作UI
        model.UpdateOperator(new List<SkillKeyUI>());
        // 显示行动提示UI
        model.SetActTipActive(true, false);
    }

    /// <summary>
    /// 更新行动条
    /// </summary>
    public async Task UpadteActionBar(IEnumerable<IBattleEntityObject> battleEntities)
    {
        List<ActionGridUI> actionGridUIs = new List<ActionGridUI>();
        bool isFirst = true;
        foreach (IBattleEntityObject battleEntity in battleEntities)
        {
            ActionGridUI actionGridUI = await ObjectBuilder.GetOrCreateInstance<ActionGridUI>(E_AssetBundleType.UI, ResKeyCollection.ActionGridUI, null);
            //Sprite icon = await AssetBundleManager.Instance.LoadAssetAsync<Sprite>(E_AssetBundleType.UI, "");
            actionGridUI.Init(null, battleEntity.ActionValue, battleEntity, isFirst);
            actionGridUIs.Add(actionGridUI);
            isFirst = false;
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
    /// <param name="selectTargetEvent"></param>
    private async void OnTargetSelectionChanged(SelectTargetEvent selectTargetEvent)
    {
        if (selectTargetEvent.MainTarget is PlayerObject)
        {
            return;
        }

        // 更新目标标记UI显示
        await UpdateTargetMarker(selectTargetEvent.SelectedTargets);
        // 更新行动轴目标高亮显示
        await UpdateActionGridHighlight(selectTargetEvent.SelectedTargets);
    }

    /// <summary>
    /// 更新目标标记
    /// </summary>
    /// <param name="selectedTargets"></param>
    private async Task UpdateTargetMarker(List<IBattleEntityObject> selectedTargets)
    {
        List<SelectMarkerUI> selectMarkerUIs = new List<SelectMarkerUI>();
        foreach (IBattleEntityObject battleEntity in selectedTargets)
        {
            SelectMarkerUI selectMarkerUI = await ObjectBuilder.GetOrCreateInstance<SelectMarkerUI>(E_AssetBundleType.UI, ResKeyCollection.SelectMarkerUI, null);
            if (UIManager.WorldToLocalPointInRectangle(BattlePoint.Instance.CurrentActiveCamera, UIManager.Instance.UICamera, view.SelectMarkerArea, selectMarkerUI.gameObject, battleEntity.GameObject.transform.position, Vector2.up * 50))
            {
                selectMarkerUI.InitSelectMarker((battleEntity is PlayerObject) ? E_SkillTargetType.Friend : E_SkillTargetType.Enemy);
                selectMarkerUIs.Add(selectMarkerUI);
            }
        }
        model.UpdateSelectMarker(selectMarkerUIs);
    }

    /// <summary>
    /// 更新行动格子高亮
    /// </summary>
    /// <param name="selectedTargets"></param>
    /// <returns></returns>
    private async Task UpdateActionGridHighlight(List<IBattleEntityObject> selectedTargets)
    {
        List<ActionGridUI> actionGridUI = model.GetActionGridUIs();

        foreach (ActionGridUI actionGrid in actionGridUI)
        {
            actionGrid.CheckSelect(null);
        }

        if (selectedTargets.Count > 1)
        {
            foreach (ActionGridUI actionGrid in actionGridUI)
            {
                foreach (IBattleEntityObject battleEntity in selectedTargets)
                {
                    if (!actionGrid.IsSelect)
                    {
                        actionGrid.CheckSelect(battleEntity);
                    }
                }
            }
        }
        else if(selectedTargets.Count == 1)
        {
            foreach (ActionGridUI actionGrid in actionGridUI)
            {
                foreach (IBattleEntityObject battleEntity in selectedTargets)
                {
                    actionGrid.CheckSelect(battleEntity);
                }
            }
        }
    }

    /// <summary>
    /// 回合开始事件监听
    /// </summary>
    /// <param name="turnStartEvent"></param>
    private async void OnTurnStart(TurnStartEvent turnStartEvent)
    {
        // 玩家相机位置不同，需要每回合开始时更新怪物UI位置
        await InitMonsterUI(turnStartEvent.Context.GetMonsterObjects());
        if (turnStartEvent.CurrentBattleEntity is PlayerObject)
        {
            // 更新当前操作UI
            UpdateOperator(turnStartEvent.CurrentBattleEntity);
        }
        else if(turnStartEvent.CurrentBattleEntity is MonsterObject)
        {
            // 显示怪物行动提示
            model.SetActTipActive(true, true);
        }
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
    private async void OnTakeDamage(TakeDamageEvent onTakeDamageEvent)
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
    private async void OnHpChanged(HpChangedEvent onHpChangedEvent)
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

    internal void BattleOver()
    {
        throw new NotImplementedException();
    }
}
