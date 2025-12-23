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

    private Vector2 damageTextXOffsetRange = new Vector2(-60, 60);
    private Vector2 damageTextYOffsetRange = new Vector2(-20, 20);

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
                roleStateUI.Init(entityObject.GetComponent<PlayerPropertyComponent>().GetProperty<RoleProperty>(), skillId);
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

    private void OnTurnEnd(TurnEndEvent turnEndEvent)
    {

    }

    /// <summary>
    /// 受到伤害回调事件
    /// </summary>
    /// <param name="onTakeDamageEvent"></param>
    private async void OnTakeDamage(OnTakeDamageEvent onTakeDamageEvent)
    {
        DamageResult damageResult = onTakeDamageEvent.DamageResult;
        DamageTextUI damageTextUI = await ObjectBuilder.GetOrCreateInstance<DamageTextUI>(E_AssetBundleType.UI, ResKeyCollection.DamageTextUI, null);
        Vector2 dmgTextOffset = new Vector2(UnityEngine.Random.Range(damageTextXOffsetRange.x, damageTextXOffsetRange.y), UnityEngine.Random.Range(damageTextYOffsetRange.x, damageTextYOffsetRange.y));
        //坐标转换，初始化
        if (UIManager.WorldToLocalPointInRectangle(Camera.main, UIManager.Instance.UICamera, view.transform, damageTextUI.gameObject, damageResult.Target.GameObject.transform.position, dmgTextOffset))
        {
            string critText = damageResult.IsCrit ? "暴击" : "";
            damageTextUI.InitDamageText(((int)damageResult.ElementType).ToElementTypeColor(), critText, damageResult.FinalDamage);
        }
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

    private async Task UpdateBattlePointCount(int current, int max)
    {
        LogManager.Log($"当前战技点数：{current}");
        List<BattlePointUI> battlePointUIs = new List<BattlePointUI>();
        for (int i = 0; i < max; i++)
        {
            BattlePointUI battlePointUI = await ObjectBuilder.GetOrCreateInstance<BattlePointUI>(E_AssetBundleType.UI, ResKeyCollection.BattlePointUI, null);
            battlePointUI.SetActivePoint(i < current);
            battlePointUIs.Add(battlePointUI);
        }

        model.UpdateBattlePointCount(current, battlePointUIs);
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
        UIManager.WorldToLocalPointInRectangle(Camera.main, UIManager.Instance.UICamera, view.transform, actingFlagObj, worldPos, Vector2.up * 125f);
    }

    internal void BattleOver()
    {
        throw new NotImplementedException();
    }
}
