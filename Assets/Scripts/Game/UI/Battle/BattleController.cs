using Game.Battle;
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
    private Vector2 damageTextXOffsetRange = new Vector2(-40, 40);
    private Vector2 damageTextYOffsetRange = new Vector2(-10, 10);

    private IBattleEntityObject currentActObject;

    // 依赖注入各子模块
    private BattleUIInitializer _uiInitializer;
    private BattleEventProcessor _eventProcessor;
    private BattleUIManager _uiManager;

    public BattleController(BattleView view, BattleModel model) : base(view, model)
    {

    }

    protected async override Task OnInit()
    {
        _uiInitializer = new BattleUIInitializer(view, model);
        _uiManager = new BattleUIManager(view, model);
        _eventProcessor = new BattleEventProcessor(this, _uiManager, _uiInitializer);
    }

    /// <summary>
    /// 初始化战斗UI
    /// </summary>
    /// <param name="battleEntities"></param>
    public async Task InitBattleUI(IBattleContext battleContext)
    {
        await _uiInitializer.InitPlayerUI(battleContext.GetPlayerObjects());
        await _uiManager.UpdateBattlePointCount(battleContext.CurentBattlePointCount, battleContext.MaxBattlePointCount);
        _eventProcessor.RegisterBattleEvents(battleContext.GetEventBus());
    }

    public BattleUIManager GetBattleUI() => _uiManager;

    ///// <summary>
    ///// 初始化玩家UI
    ///// </summary>
    ///// <param name="battleEntities"></param>
    ///// <returns></returns>
    //private async Task InitPlayerUI(IEnumerable<IBattleEntityObject> battleEntities)
    //{
    //    List<RoleStateUI> roleStateUIs = new List<RoleStateUI>();
    //    // 玩家角色显示UI
    //    foreach (IBattleEntityObject battleEntity in battleEntities)
    //    {
    //        RoleStateUI roleStateUI = await ObjectBuilder.GetOrCreateInstance<RoleStateUI>(E_AssetBundleType.UI, ResKeyCollection.RoleStateUI, null);
    //        int skillId = battleEntity.GetComponent<SkillComponent>().GetUltimateSkill();
    //        if (skillId != -1)
    //        {
    //            Sprite icon = await AssetBundleManager.Instance.LoadAssetAsync<Sprite>(E_AssetBundleType.Texture, ResKeyCollection.WhiteImage); 
    //            roleStateUI.Init(battleEntity.GetComponent<PlayerPropertyComponent>().GetProperty<RoleProperty>(), icon, skillId, battleEntity);
    //            roleStateUIs.Add(roleStateUI);
    //        }
    //    }

    //    model.InitRoleStateUI(roleStateUIs);
    //}

    ///// <summary>
    ///// 初始化怪物UI
    ///// 依赖玩家相机初始化完毕
    ///// </summary>
    ///// <param name="battleEntities"></param>
    ///// <returns></returns>
    //private async Task InitMonsterUI(IEnumerable<IBattleEntityObject> battleEntities)
    //{
    //    List<NormalMonsterStateUI> normalMonsterStateUIs = new List<NormalMonsterStateUI>(); 
    //    // 怪物血量UI
    //    foreach (IBattleEntityObject battleEntity in battleEntities)
    //    {
    //        NormalMonsterStateUI monsterStateUI = await ObjectBuilder.GetOrCreateInstance<NormalMonsterStateUI>(E_AssetBundleType.UI, ResKeyCollection.MonsterStateUI, null);
    //        if (UIManager.WorldToLocalPointInRectangle(BattlePoint.Instance.CurrentActiveCamera, UIManager.Instance.UICamera, view.MonsterStateArea, monsterStateUI.gameObject, battleEntity.GameObject.transform.position, Vector2.up * 250))
    //        {
    //            monsterStateUI.Init(battleEntity);
    //            normalMonsterStateUIs.Add(monsterStateUI);
    //        }
    //    }

    //    model.UpdateNormalMonsterState(normalMonsterStateUIs);
    //}

    ///// <summary>
    ///// 玩家触发技能事件回调
    ///// </summary>
    ///// <param name="playerTriggerSkillEvent"></param>
    //private void OnPlayerTriggerSkill(PlayerTriggerSkillEvent playerTriggerSkillEvent)
    //{
    //    HideOperator(false);
    //}

    ///// <summary>
    ///// 隐藏相关操作
    ///// 更新行动提示、失活目标选择、隐藏玩家操作
    ///// </summary>
    //public void HideOperator(bool isMonster)
    //{
    //    // 失活目标选择
    //    ServiceLocator.Instance.Get<ITargetSelectManager>().InActiveSelectTarget();
    //    // 清除标记UI
    //    model.ClearSelectMarker();
    //    // 隐藏玩家操作UI
    //    model.UpdateOperator(new List<SkillKeyUI>());
    //    // 显示行动提示UI
    //    model.SetActTipActive(true, isMonster);
    //}

    ///// <summary>
    ///// 显示终结技界面事件回调
    ///// </summary>
    ///// <param name="showUltimateUIEvent"></param>
    //private void OnShowUltimateUIEvent(ShowUltimateUIEvent showUltimateUIEvent)
    //{
    //    ServiceLocator.Instance.Get<IMonoManager>().StartCoroutine(WaitForPaitingOver(showUltimateUIEvent.Skill.SkillInfo));
    //    // 更新终结技UI显示
    //    UpdateOperator(showUltimateUIEvent.Caster, SkillKeyUIDataProviderFactory.GetProvider<UltimateSkillKeyUIDataProvider>());
    //}

    //private IEnumerator WaitForPaitingOver(SkillInfo skillInfo)
    //{
    //    // 显示角色立绘
    //    model.SetUltimatePaitingActive(true, null, skillInfo.f_name);
    //    // 显示一秒后隐藏
    //    yield return new WaitForSeconds(1f);
    //    model.SetUltimatePaitingActive(false, null, string.Empty);
    //}

    ///// <summary>
    ///// 终结技释放结束事件回调
    ///// 用于恢复当前行动角色操作UI
    ///// </summary>
    ///// <param name="ultimateReleaseOverEvent"></param>
    //private void OnUltimateReleaseOverEvent(UltimateReleaseOverEvent ultimateReleaseOverEvent)
    //{
    //    if (currentActObject is not PlayerObject)
    //    {
    //        return;
    //    }

    //    UpdateOperator(currentActObject, SkillKeyUIDataProviderFactory.GetProvider<BaseSkillKeyUIDataProvider>());
    //}

    ///// <summary>
    ///// 更新行动条
    ///// </summary>
    //public async Task UpdateActionBar(IEnumerable<IBattleEntityObject> battleEntities)
    //{
    //    List<ActionGridUI> actionGridUIs = new List<ActionGridUI>();
    //    bool isFirst = true;
    //    foreach (IBattleEntityObject battleEntity in battleEntities)
    //    {
    //        ActionGridUI actionGridUI = await ObjectBuilder.GetOrCreateInstance<ActionGridUI>(E_AssetBundleType.UI, ResKeyCollection.ActionGridUI, null);
    //        //Sprite icon = await AssetBundleManager.Instance.LoadAssetAsync<Sprite>(E_AssetBundleType.UI, "");
    //        actionGridUI.Init(null, battleEntity.ActionValue, battleEntity, isFirst);
    //        actionGridUIs.Add(actionGridUI);
    //        isFirst = false;
    //    }
    //    model.UpdateAcitonbar(actionGridUIs);
    //}

    ///// <summary>
    ///// 更新指定玩家操作UI
    ///// </summary>
    ///// <param name="currentObject"></param>
    //private async void UpdateOperator(IBattleEntityObject currentObject, ISkillKeyUIDataProvider dataProvider)
    //{
    //    // 隐藏行动提示
    //    model.SetActTipActive(false, false);
    //    List<SkillKeyUI> skillKeyUIs = new List<SkillKeyUI>();
    //    SkillKeyUIData skillKeyUIData = dataProvider.GetData(currentObject);
    //    var infos = skillKeyUIData.SkillInfos;
    //    foreach (SkillInfo info in infos)
    //    {
    //        // 玩家操作UI
    //        SkillKeyUI skillKeyUI = await ObjectBuilder.GetOrCreateInstance<SkillKeyUI>(E_AssetBundleType.UI, ResKeyCollection.SkillKeyUI, null);
    //        skillKeyUI.Init(info, view.SkillKeyGroup, currentObject);
    //        skillKeyUIs.Add(skillKeyUI);
    //    }

    //    model.UpdateOperator(skillKeyUIs);
    //}

    ///// <summary>
    ///// 目标选择变化事件回调
    ///// </summary>
    ///// <param name="selectTargetEvent"></param>
    //private async void OnTargetSelectionChanged(SelectTargetEvent selectTargetEvent)
    //{
    //    if (selectTargetEvent.MainTarget is PlayerObject)
    //    {
    //        return;
    //    }

    //    // 更新目标标记UI显示
    //    await UpdateTargetMarker(selectTargetEvent.SelectedTargets);
    //    // 更新行动轴目标高亮显示
    //    UpdateActionGridHighlight(selectTargetEvent.SelectedTargets);
    //}

    ///// <summary>
    ///// 更新目标标记
    ///// </summary>
    ///// <param name="selectedTargets"></param>
    //private async Task UpdateTargetMarker(List<IBattleEntityObject> selectedTargets)
    //{
    //    List<SelectMarkerUI> selectMarkerUIs = new List<SelectMarkerUI>();
    //    foreach (IBattleEntityObject battleEntity in selectedTargets)
    //    {
    //        SelectMarkerUI selectMarkerUI = await ObjectBuilder.GetOrCreateInstance<SelectMarkerUI>(E_AssetBundleType.UI, ResKeyCollection.SelectMarkerUI, null);
    //        if (UIManager.WorldToLocalPointInRectangle(BattlePoint.Instance.CurrentActiveCamera, UIManager.Instance.UICamera, view.SelectMarkerArea, selectMarkerUI.gameObject, battleEntity.GameObject.transform.position, Vector2.up * 50))
    //        {
    //            selectMarkerUI.InitSelectMarker((battleEntity is PlayerObject) ? E_SkillTargetType.Friend : E_SkillTargetType.Enemy);
    //            selectMarkerUIs.Add(selectMarkerUI);
    //        }
    //    }
    //    model.UpdateSelectMarker(selectMarkerUIs);
    //}

    ///// <summary>
    ///// 更新行动格子高亮
    ///// </summary>
    ///// <param name="selectedTargets"></param>
    ///// <returns></returns>
    //private void UpdateActionGridHighlight(List<IBattleEntityObject> selectedTargets)
    //{
    //    List<ActionGridUI> actionGridUI = model.GetActionGridUIs();

    //    foreach (ActionGridUI actionGrid in actionGridUI)
    //    {
    //        actionGrid.CheckSelect(null);
    //    }

    //    if (selectedTargets.Count > 1)
    //    {
    //        foreach (ActionGridUI actionGrid in actionGridUI)
    //        {
    //            foreach (IBattleEntityObject battleEntity in selectedTargets)
    //            {
    //                if (!actionGrid.IsSelect)
    //                {
    //                    actionGrid.CheckSelect(battleEntity);
    //                }
    //            }
    //        }
    //    }
    //    else if(selectedTargets.Count == 1)
    //    {
    //        foreach (ActionGridUI actionGrid in actionGridUI)
    //        {
    //            foreach (IBattleEntityObject battleEntity in selectedTargets)
    //            {
    //                actionGrid.CheckSelect(battleEntity);
    //            }
    //        }
    //    }
    //}

    ///// <summary>
    ///// 回合开始事件监听
    ///// </summary>
    ///// <param name="turnStartEvent"></param>
    //private async void OnTurnStart(TurnStartEvent turnStartEvent)
    //{
    //    // 记录当前行动角色
    //    currentActObject = turnStartEvent.CurrentBattleEntity;

    //    // 玩家相机位置不同，需要每回合开始时更新怪物UI位置
    //    await InitMonsterUI(turnStartEvent.Context.GetMonsterObjects());
    //    if (turnStartEvent.CurrentBattleEntity is PlayerObject)
    //    {
    //        // 更新当前操作UI
    //        UpdateOperator(turnStartEvent.CurrentBattleEntity, SkillKeyUIDataProviderFactory.GetProvider<BaseSkillKeyUIDataProvider>());
    //    }
    //    else if(turnStartEvent.CurrentBattleEntity is MonsterObject)
    //    {
    //        HideOperator(true);
    //    }
    //}

    //public void ClearDamageTextUI()
    //{
    //    model.UpdateCumulativeDamage(false, 0);
    //}

    ///// <summary>
    ///// 回合结束事件监听
    ///// </summary>
    ///// <param name="turnEndEvent"></param>
    //private void OnTurnEnd(TurnEndEvent turnEndEvent)
    //{
    //    // TODO：不是回合结束，而是造成伤害的指令结束后清空
    //    model.UpdateCumulativeDamage(false, 0);
    //}

    ///// <summary>
    ///// 命令等待事件
    ///// 更新命令排队UI
    ///// </summary>
    ///// <param name="commandWaitEvent"></param>
    //private async void OnCommandWaitEvent(CommandWaitEvent commandWaitEvent)
    //{
    //    List<WaitingActUI> waitingActUIs = new List<WaitingActUI>();
    //    foreach (var item in commandWaitEvent.WaitingSkills)
    //    {
    //        WaitingActUI waitingActUI = await ObjectBuilder.GetOrCreateInstance<WaitingActUI>(E_AssetBundleType.UI, ResKeyCollection.WaitingActUI, null);
    //        waitingActUI.Init(null);
    //        waitingActUIs.Add(waitingActUI);
    //    }

    //    model.UpdateWaitingCommmand(waitingActUIs);
    //}

    /////// <summary>
    /////// 受到伤害事件回调
    /////// 显示伤害文本
    /////// </summary>
    /////// <param name="applyDamageEvent"></param>
    ////private async void OnTakeDamage(ApplyDamageEvent applyDamageEvent)
    ////{
    ////    DamageResult damageResult = applyDamageEvent.DamageResult;
    ////    DamageTextUI damageTextUI = await ObjectBuilder.GetOrCreateInstance<DamageTextUI>(E_AssetBundleType.UI, ResKeyCollection.DamageTextUI, null);
    ////    Vector2 dmgTextOffset = GetDamageTextUIPos(damageResult.Target, damageTextXOffsetRange, damageTextYOffsetRange);
    ////    // 坐标转换，初始化
    ////    if (UIManager.WorldToLocalPointInRectangle(BattlePoint.Instance.CurrentActiveCamera, UIManager.Instance.UICamera, view.transform, damageTextUI.gameObject, damageResult.Target.GameObject.transform.position, dmgTextOffset))
    ////    {
    ////        damageTextUI.InitDamageText(((int)damageResult.ElementType).ToElementTypeColor(), this.GetDamgeTypeText(damageResult), damageResult.FinalDamage);
    ////    }
    ////    // 更新累计伤害
    ////    model.UpdateCumulativeDamage(true, damageResult.FinalDamage);
    ////}

    /////// <summary>
    /////// 血量变化回调事件
    /////// 显示恢复文本
    /////// </summary>
    /////// <param name="hpChangedEvent"></param>
    ////private async void OnHpChanged(HpChangedEvent hpChangedEvent)
    ////{
    ////    if (hpChangedEvent.DeltaHp < 0)
    ////    {
    ////        DamageTextUI damageTextUI = await ObjectBuilder.GetOrCreateInstance<DamageTextUI>(E_AssetBundleType.UI, ResKeyCollection.HealTextUI, null);
    ////        Vector2 dmgTextOffset = this.GetDamageTextUIPos(hpChangedEvent.Target, damageTextXOffsetRange, damageTextYOffsetRange);
    ////        // 坐标转换，初始化
    ////        if (UIManager.WorldToLocalPointInRectangle(BattlePoint.Instance.CurrentActiveCamera, UIManager.Instance.UICamera, view.transform, damageTextUI.gameObject, hpChangedEvent.Target.GameObject.transform.position, dmgTextOffset))
    ////        {
    ////            damageTextUI.InitDamageText(Color.green, this.GetHealText(), hpChangedEvent.DeltaHp);
    ////        }
    ////    }
    ////}

    ///// <summary>
    ///// 护盾变化事件回调
    ///// 显示护盾变化文本
    ///// </summary>
    ///// <param name="onShieldChangedEvent"></param>
    //private void OnShieldChanged(ShieldChangedEvent onShieldChangedEvent)
    //{
    //    if (onShieldChangedEvent.DeltaShield < 0)
    //    {
    //        // 护盾增加显示

    //    }
    //    else
    //    {
    //        // 护盾减少显示

    //    }
    //}

    ///// <summary>
    ///// 战技点变化事件
    ///// </summary>
    ///// <param name="battlePointCountChanged"></param>
    //private async void OnBattlePointCountChanged(OnBattlePointCountChangedEvent battlePointCountChanged)
    //{
    //    await UpdateBattlePointCount(battlePointCountChanged.CurentBattlePointCount, battlePointCountChanged.MaxBattlePointCount);
    //}

    ///// <summary>
    ///// 更新战技点数
    ///// </summary>
    ///// <param name="current"></param>
    ///// <param name="max"></param>
    ///// <returns></returns>
    //private async Task UpdateBattlePointCount(int current, int max)
    //{
    //    List<BattlePointUI> battlePointUIs = new List<BattlePointUI>();
    //    for (int i = 0; i < max; i++)
    //    {
    //        BattlePointUI battlePointUI = await ObjectBuilder.GetOrCreateInstance<BattlePointUI>(E_AssetBundleType.UI, ResKeyCollection.BattlePointUI, null);
    //        battlePointUI.SetActivePoint(i < current);
    //        battlePointUIs.Add(battlePointUI);
    //    }

    //    model.UpdateBattlePointCount(current, battlePointUIs);
    //}

    //public void BattleOver()
    //{

    //}
}
