using Framework;
using Game;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 战斗界面UI管理器
/// </summary>
public class BattleUIManager
{
    /// <summary>
    /// 行动提示类型
    /// 用于战斗界面右下角显示
    /// </summary>
    public enum E_ActTipType : byte
    {
        /// <summary>
        /// 隐藏
        /// </summary>
        Hide,
        /// <summary>
        /// 玩家行动
        /// </summary>
        Player,
        /// <summary>
        /// 怪物行动
        /// </summary>
        Monster,
    }

    private BattleView _view;
    private BattleModel _model;
    private Vector2 damageTextXOffsetRange = new Vector2(-40, 40);
    private Vector2 damageTextYOffsetRange = new Vector2(-10, 10);

    // 战斗结束UI相关
    private static WaitForSeconds _waitForSeconds0_5 = new WaitForSeconds(0.5f);
    private static WaitForSeconds _waitForSeconds2_5 = new WaitForSeconds(2.5f);

    public BattleUIManager(BattleView view, BattleModel model)
    {
        _view = view;
        _model = model;
    }

    /// <summary>
    /// 隐藏普通怪物状态UI
    /// </summary>
    /// <param name="deadMonster"></param>
    public void HideNormalMonsterStateUI(IBattleEntityObject deadMonster)
    {
        _model.HideNormalMonsterStateUI(deadMonster);
    }

    /// <summary>
    /// 显示战斗结束UI
    /// </summary>
    public void ShowBattleOver(IBattleContext context)
    {
        ServiceLocator.Get<IMonoManager>().StartCoroutine(ShowBattleOver_Cor());

        IEnumerator ShowBattleOver_Cor()
        {
            _view.BattleOverArea.gameObject.SetActive(true);

            yield return _waitForSeconds2_5;

            _view.BattleOverArea.gameObject.SetActive(false);

            yield return _waitForSeconds0_5;

            // 触发退出战斗事件
            context.GetEventBus().TriggerEvent(new QuitBattleEvent(context));
        }
    }

    /// <summary>
    /// 显示战斗消息提示
    /// </summary>
    /// <param name="msg"></param>
    public async void ShowBattleMessage(string msg)
    {
        BattleMessageUI battleMessageUI = await ObjectBuilder.GetObject<BattleMessageUI>(E_AssetBundleType.UI, ResKeyCollection.BattleMessageUI, _view.BattleMsgArea);
        battleMessageUI.InitMessage(Color.red, msg);
    }

    /// <summary>
    /// 显示伤害文本
    /// </summary>
    public async void ShowDamageText(DamageResult damageResult)
    {
        DamageTextUI damageTextUI = await ObjectBuilder.GetObject<DamageTextUI>(E_AssetBundleType.UI, ResKeyCollection.DamageTextUI, null);
        // 获取伤害文本显示的位置
        Vector2 dmgTextOffset = GetDamageTextUIPos(damageResult.Target, damageTextXOffsetRange, damageTextYOffsetRange);
        // 世界转UI坐标
        if (UIManager.WorldToLocalPointInRectangle(BattlePoint.Instance.CurrentActiveCamera, UIManager.Instance.UICamera, _view.transform, damageTextUI.gameObject, damageResult.Target.GameObject.transform.position, dmgTextOffset))
        {
            // 初始化
            damageTextUI.InitDamageText(((int)damageResult.ElementType).ToElementTypeColor(), GetDamgeTypeText(damageResult), damageResult.FinalDamage);
        }
        // 更新累计伤害
        UpdateCumulativeDamage(true, damageResult.FinalDamage);
    }

    /// <summary>
    /// 显示治疗文本
    /// </summary>
    /// <param name="target"></param>
    /// <param name="deltaHp"></param>
    public async void ShowHealText(IBattleEntityObject target, int deltaHp)
    {
        DamageTextUI damageTextUI = await ObjectBuilder.GetObject<DamageTextUI>(E_AssetBundleType.UI, ResKeyCollection.HealTextUI, null);
        Vector2 dmgTextOffset = GetDamageTextUIPos(target, damageTextXOffsetRange, damageTextYOffsetRange);
        // 坐标转换，初始化
        if (UIManager.WorldToLocalPointInRectangle(BattlePoint.Instance.CurrentActiveCamera, UIManager.Instance.UICamera, _view.transform, damageTextUI.gameObject, target.GameObject.transform.position, dmgTextOffset))
        {
            damageTextUI.InitDamageText(Color.green, GetHealText(), deltaHp);
        }
    }

    /// <summary>
    /// 更新累计伤害UI
    /// </summary>
    /// <param name="isShow"></param>
    /// <param name="dmg"></param>
    public void UpdateCumulativeDamage(bool isShow, int dmg)
    {
        _view.TotalDmgArea.gameObject.SetActive(isShow);
        _view.UpdateTotalDmg(_model.SetCumulativeDamage(dmg, !isShow));
    }

    /// <summary>
    /// 更新等待命令UI
    /// </summary>
    /// <param name="iconPaths">命令要显示的图标路径列表</param>
    public async void UpdateWaitingCommmand(List<string> iconPaths)
    {
        List<WaitingActUI> waitingActUIs = new List<WaitingActUI>();
        foreach (string iconPath in iconPaths)
        {
            WaitingActUI waitingActUI = await ObjectBuilder.GetObject<WaitingActUI>(E_AssetBundleType.UI, ResKeyCollection.WaitingActUI, _view.WaitQueueContent);
            Sprite icon = await AssetBundleManager.Instance.LoadAssetAsync<Sprite>(E_AssetBundleType.Texture, iconPath);
            waitingActUI.Init(icon);
            waitingActUIs.Add(waitingActUI);
        }

        _model.UpdateWaitingCommmand(waitingActUIs);
    }

    /// <summary>
    /// 显示角色立绘
    /// 触发终结技后显示
    /// 内部通过一个协程处理
    /// </summary>
    /// <param name="skillInfo"></param>
    /// <returns></returns>
    public void ShowPaiting(SkillInfo skillInfo)
    {
        ServiceLocator.Get<IMonoManager>().StartCoroutine(ShowPaiting_Cor(skillInfo));

        // 显示角色立绘本地协程函数
        IEnumerator ShowPaiting_Cor(SkillInfo skillInfo)
        {
            // 显示角色立绘
            _view.UpdateUltimateShow(true, null, skillInfo.f_name);
            // 显示一秒后隐藏
            yield return new WaitForSeconds(1f);
            _view.UpdateUltimateShow(false, null, string.Empty);
        }
    }

    /// <summary>
    /// 更新行动条UI
    /// 更新行动条里的行动格子UI
    /// </summary>
    public async void UpdateActionBar(IEnumerable<IBattleEntityObject> battleEntities)
    {
        List<ActionGridUI> actionGridUIs = new List<ActionGridUI>();
        // 第一个格子需要放大处理
        bool isFirst = true;
        foreach (IBattleEntityObject battleEntity in battleEntities)
        {
            ActionGridUI actionGridUI = await ObjectBuilder.GetObject<ActionGridUI>(E_AssetBundleType.UI, ResKeyCollection.ActionGridUI, _view.ActionBarContent);
            //Sprite icon = await AssetBundleManager.Instance.LoadAssetAsync<Sprite>(E_AssetBundleType.UI, "");
            actionGridUI.Init(null, battleEntity.ActionValue, battleEntity, isFirst);
            actionGridUIs.Add(actionGridUI);
            isFirst = false;
        }
        _model.UpdateAcitonbar(actionGridUIs);
    }

    /// <summary>
    /// 清除目标标记
    /// </summary>
    public void ClearSelectMarker()
    {
        // 清除激活的标记UI
        _model.ClearSelectMarker();
    }

    /// <summary>
    /// 设置操作UI
    /// 传null为隐藏
    /// </summary>
    /// <param name="skillKeyUIs"></param>
    public void SetOperator(List<SkillKeyUI> skillKeyUIs)
    {
        if (skillKeyUIs == null)
        {
            _model.ClearOperator();
            return;
        }
        // 设置操作UI
        _model.SetOperator(skillKeyUIs);
    }

    /// <summary>
    /// 设置行动提示激活状态
    /// 需要先SetOperator隐藏按键UI
    /// </summary>
    /// <param name="actTipType"></param>
    public void SetActTipActive(E_ActTipType actTipType)
    {
        bool isActive = actTipType != E_ActTipType.Hide;
        _view.ActingTipUI.gameObject.SetActive(isActive);
        if (isActive)
        {
            _view.ActingTipUI.UpdateTipText(actTipType == E_ActTipType.Monster);
        }
    }

    /// <summary>
    /// 更新战技点数
    /// </summary>
    /// <param name="current"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public async Task UpdateBattlePointCount(int current, int max)
    {
        List<BattlePointUI> battlePointUIs = new List<BattlePointUI>();
        for (int i = 0; i < max; i++)
        {
            BattlePointUI battlePointUI = await ObjectBuilder.GetObject<BattlePointUI>(E_AssetBundleType.UI, ResKeyCollection.BattlePointUI, _view.PointContent);
            battlePointUI.SetActivePoint(i < current);
            battlePointUIs.Add(battlePointUI);
        }
        // 更新UI数据
        _model.UpdateBattlePointCount(current, battlePointUIs);
        _view.UpdateBattlePointCount(current);
    }

    /// <summary>
    /// 更新指定玩家操作UI
    /// 更新操作前需要调用SetActTipActive隐藏行动提示
    /// </summary>
    /// <param name="currentObject"></param>
    /// <param name="dataProvider"></param>
    public async void UpdateOperator(IBattleEntityObject currentObject, ISkillKeyUIDataProvider dataProvider)
    {
        List<SkillKeyUI> skillKeyUIs = new List<SkillKeyUI>();
        SkillKeyUIData skillKeyUIData = dataProvider.GetData(currentObject);
        var infos = skillKeyUIData.SkillInfos;
        foreach (SkillInfo info in infos)
        {
            SkillKeyUI skillKeyUI = await ObjectBuilder.GetObject<SkillKeyUI>(E_AssetBundleType.UI, ResKeyCollection.SkillKeyUI, _view.OperatorArea);
            skillKeyUI.Init(info, _view.SkillKeyGroup, currentObject);
            skillKeyUIs.Add(skillKeyUI);
        }
        SetOperator(skillKeyUIs);
    }

    /// <summary>
    /// 更新目标标记
    /// 传null为隐藏
    /// </summary>
    /// <param name="selectedTargets"></param>
    public async void UpdateTargetMarker(List<IBattleEntityObject> selectedTargets)
    {
        if (selectedTargets == null)
        {
            _model.ClearSelectMarker();
            return;
        }

        List<SelectMarkerUI> selectMarkerUIs = new List<SelectMarkerUI>();
        foreach (IBattleEntityObject battleEntity in selectedTargets)
        {
            // 不用设置父对象，在坐标转换时会自动设置
            SelectMarkerUI selectMarkerUI = await ObjectBuilder.GetObject<SelectMarkerUI>(E_AssetBundleType.UI, ResKeyCollection.SelectMarkerUI, null);
            if (UIManager.WorldToLocalPointInRectangle(BattlePoint.Instance.CurrentActiveCamera, UIManager.Instance.UICamera, _view.SelectMarkerArea, selectMarkerUI.gameObject, battleEntity.GameObject.transform.position, Vector2.up * 50))
            {
                selectMarkerUI.InitSelectMarker((battleEntity is PlayerObject) ? E_SkillTargetType.Friend : E_SkillTargetType.Enemy);
                selectMarkerUIs.Add(selectMarkerUI);
            }
        }
        _model.UpdateSelectMarker(selectMarkerUIs);
    }

    /// <summary>
    /// 更新选中目标的行动格子高亮
    /// </summary>
    /// <param name="selectedTargets"></param>
    /// <returns></returns>
    public void UpdateActionGridHighlight(List<IBattleEntityObject> selectedTargets)
    {
        List<ActionGridUI> actionGridUI = _model.GetActionGridUIs();

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
        else if (selectedTargets.Count == 1)
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
    /// 更新玩家状态栏
    /// </summary>
    /// <param name="currentBattleEntity"></param>
    public void UpdatePlayerStatuebar(IBattleEntityObject currentBattleEntity)
    {
        // 获取指定角色的状态UI
        RoleStateUI roleStateUI = _model.GetRoleStateUIById(currentBattleEntity.BattleEntityId);
        if (roleStateUI != null)
        {
            roleStateUI.UpdateStatus();
        }
    }

    /// <summary>
    /// 显示状态文本效果
    /// 实体被添加状态时显示状态飘字效果提示
    /// </summary>
    /// <param name="newStatus"></param>
    public async void ShowStatusText(IStatus newStatus)
    {
        // 不用设置父对象，坐标转换中会设置
        StatusEffectTextUI statusEffectTextUI = await ObjectBuilder.GetObject<StatusEffectTextUI>(E_AssetBundleType.UI, ResKeyCollection.StatusEffectTextUI, null);
        if (UIManager.WorldToLocalPointInRectangle(BattlePoint.Instance.CurrentActiveCamera, ServiceLocator.Get<IUIManager>().UICamera,
            _view.BuffTextArea, statusEffectTextUI.gameObject, newStatus.Owner.SubGameObject.transform.position, Vector2.up * 160))
        {
            statusEffectTextUI.InitText(null, newStatus.StatusProperty.StatusInfo.f_name);
        }
    }

    /// <summary>
    /// 清理活跃的伤害文本UI
    /// </summary>
    public void ClearActiveDamageTextUI()
    {
        _view.TotalDmgArea.gameObject.SetActive(false);
        _view.UpdateTotalDmg(_model.SetCumulativeDamage(0, true));
    }

    /// <summary>
    /// 获取伤害文本UI位置
    /// </summary>
    /// <param name="_"></param>
    /// <param name="dmgTarget"></param>
    /// <param name="damageTextXOffsetRange"></param>
    /// <param name="damageTextYOffsetRange"></param>
    /// <returns></returns>
    public static Vector2 GetDamageTextUIPos(IBattleEntityObject dmgTarget, Vector2 damageTextXOffsetRange, Vector2 damageTextYOffsetRange)
    {
        float x = Random.Range(damageTextXOffsetRange.x, damageTextXOffsetRange.y);
        float y = Random.Range(damageTextYOffsetRange.x, damageTextYOffsetRange.y);
        Vector2 dmgTextOffset = new Vector2(x, y);
        Vector2 pos = default;
        switch (dmgTarget)
        {
            case MonsterObject monster:
                pos = Vector2.up * monster.MonsterInfo.f_dmgTextYOffset + dmgTextOffset;
                break;
            case PlayerObject player:
                pos = Vector2.up * player.RoleInfo.f_dmgTextYOffset + dmgTextOffset;
                break;
        }
        return pos;
    }

    /// <summary>
    /// 获取伤害类型文本
    /// </summary>
    /// <param name="_"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static string GetDamgeTypeText(DamageResult result)
    {
        string dmgTypeText = string.Empty;
        if (result.DamageType == E_DamageType.Direct)
        {
            dmgTypeText = result.IsCrit ? "暴击" : "";
        }
        else
        {
            switch (result.DamageType)
            {
                case E_DamageType.True:
                    dmgTypeText = "真伤";
                    break;
                case E_DamageType.Break:
                    dmgTypeText = "击破";
                    break;
                case E_DamageType.SuperBreak:
                    dmgTypeText = "超击破";
                    break;
                case E_DamageType.Dot:
                    dmgTypeText = "持续伤害";
                    break;
            }
        }

        return dmgTypeText;
    }

    public static string GetHealText()
    {
        return "+";
    }
}
