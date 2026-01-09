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
    private BattleView _view;
    private BattleModel _model;
    private Vector2 damageTextXOffsetRange = new Vector2(-40, 40);
    private Vector2 damageTextYOffsetRange = new Vector2(-10, 10);

    public BattleUIManager(BattleView view, BattleModel model)
    {
        _view = view;
        _model = model;
    }

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
        _model.UpdateCumulativeDamage(true, damageResult.FinalDamage);
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
        _model.UpdateCumulativeDamage(isShow, dmg);
    }

    /// <summary>
    /// 更新等待命令UI
    /// </summary>
    /// <param name="waitingSkills"></param>
    public async void UpdateWaitingCommmand(List<string> iconPaths)
    {
        List<WaitingActUI> waitingActUIs = new List<WaitingActUI>();
        foreach (string iconPath in iconPaths)
        {
            WaitingActUI waitingActUI = await ObjectBuilder.GetObject<WaitingActUI>(E_AssetBundleType.UI, ResKeyCollection.WaitingActUI, null);
            Sprite icon = await AssetBundleManager.Instance.LoadAssetAsync<Sprite>(E_AssetBundleType.Texture, iconPath);
            waitingActUI.Init(icon);
            waitingActUIs.Add(waitingActUI);
        }
        _model.UpdateWaitingCommmand(waitingActUIs);
    }

    /// <summary>
    /// 设置终结技立绘显示状态
    /// </summary>
    /// <param name="isShow"></param>
    /// <param name="icon"></param>
    /// <param name="tip"></param>
    public void SetUltimatePaitingActive(bool isShow, Sprite icon, string tip)
    {
        _model.SetUltimatePaitingActive(isShow, icon, tip);
    }

    /// <summary>
    /// 更新行动条
    /// </summary>
    public async Task UpdateActionBar(IEnumerable<IBattleEntityObject> battleEntities)
    {
        List<ActionGridUI> actionGridUIs = new List<ActionGridUI>();
        bool isFirst = true;
        foreach (IBattleEntityObject battleEntity in battleEntities)
        {
            ActionGridUI actionGridUI = await ObjectBuilder.GetObject<ActionGridUI>(E_AssetBundleType.UI, ResKeyCollection.ActionGridUI, null);
            //Sprite icon = await AssetBundleManager.Instance.LoadAssetAsync<Sprite>(E_AssetBundleType.UI, "");
            actionGridUI.Init(null, battleEntity.ActionValue, battleEntity, isFirst);
            actionGridUIs.Add(actionGridUI);
            isFirst = false;
        }
        _model.UpdateAcitonbar(actionGridUIs);
    }

    /// <summary>
    /// 隐藏相关操作
    /// 更新行动提示、失活目标选择、隐藏玩家操作
    /// </summary>
    public void HideOperator(bool isMonster)
    {
        // 失活目标选择
        ServiceLocator.Instance.Get<ITargetSelectManager>().InActiveSelectTarget();
        // 清除标记UI
        _model.ClearSelectMarker();
        // 隐藏玩家操作UI
        _model.UpdateOperator(new List<SkillKeyUI>());
        // 显示行动提示UI
        _model.SetActTipActive(true, isMonster);
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
            BattlePointUI battlePointUI = await ObjectBuilder.GetObject<BattlePointUI>(E_AssetBundleType.UI, ResKeyCollection.BattlePointUI, null);
            battlePointUI.SetActivePoint(i < current);
            battlePointUIs.Add(battlePointUI);
        }

        _model.UpdateBattlePointCount(current, battlePointUIs);
    }

    /// <summary>
    /// 更新指定玩家操作UI
    /// </summary>
    /// <param name="currentObject"></param>
    public async void UpdateOperator(IBattleEntityObject currentObject, ISkillKeyUIDataProvider dataProvider)
    {
        // 隐藏行动提示
        _model.SetActTipActive(false, false);
        List<SkillKeyUI> skillKeyUIs = new List<SkillKeyUI>();
        SkillKeyUIData skillKeyUIData = dataProvider.GetData(currentObject);
        var infos = skillKeyUIData.SkillInfos;
        foreach (SkillInfo info in infos)
        {
            // 玩家操作UI
            SkillKeyUI skillKeyUI = await ObjectBuilder.GetObject<SkillKeyUI>(E_AssetBundleType.UI, ResKeyCollection.SkillKeyUI, null);
            skillKeyUI.Init(info, _view.SkillKeyGroup, currentObject);
            skillKeyUIs.Add(skillKeyUI);
        }

        _model.UpdateOperator(skillKeyUIs);
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
        ServiceLocator.Instance.Get<IMonoManager>().StartCoroutine(ShowPaiting_Cor(skillInfo));

        // 显示角色立绘本地协程函数
        IEnumerator ShowPaiting_Cor(SkillInfo skillInfo)
        {
            // 显示角色立绘
            SetUltimatePaitingActive(true, null, skillInfo.f_name);
            // 显示一秒后隐藏
            yield return new WaitForSeconds(1f);
            SetUltimatePaitingActive(false, null, string.Empty);
        }
    }

    /// <summary>
    /// 更新目标标记
    /// </summary>
    /// <param name="selectedTargets">传null为隐藏</param>
    public async Task UpdateTargetMarker(List<IBattleEntityObject> selectedTargets)
    {
        List<SelectMarkerUI> selectMarkerUIs = new List<SelectMarkerUI>();
        if (selectedTargets != null)
        {
            foreach (IBattleEntityObject battleEntity in selectedTargets)
            {
                SelectMarkerUI selectMarkerUI = await ObjectBuilder.GetObject<SelectMarkerUI>(E_AssetBundleType.UI, ResKeyCollection.SelectMarkerUI, null);
                if (UIManager.WorldToLocalPointInRectangle(BattlePoint.Instance.CurrentActiveCamera, UIManager.Instance.UICamera, _view.SelectMarkerArea, selectMarkerUI.gameObject, battleEntity.GameObject.transform.position, Vector2.up * 50))
                {
                    selectMarkerUI.InitSelectMarker((battleEntity is PlayerObject) ? E_SkillTargetType.Friend : E_SkillTargetType.Enemy);
                    selectMarkerUIs.Add(selectMarkerUI);
                }
            }
        }
        _model.UpdateSelectMarker(selectMarkerUIs);
    }

    /// <summary>
    /// 更新行动格子高亮
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
    /// 更新状态栏
    /// </summary>
    /// <param name="currentBattleEntity"></param>
    public void UpdateStatuebar(IBattleEntityObject currentBattleEntity)
    {
        RoleStateUI roleStateUI = _model.GetRoleStateUIById(currentBattleEntity.BattleEntityId);
        if (roleStateUI != null)
        {
            roleStateUI.UpdateStatus();
        }
    }


    /// <summary>
    /// 清理活跃的伤害文本UI
    /// </summary>
    public void ClearActiveDamageTextUI()
    {
        _model.UpdateCumulativeDamage(false, 0);
    }

    public void BattleOver()
    {

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
