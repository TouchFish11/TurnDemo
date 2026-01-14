using Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleView : UIView
{
    [Inject] private ScrollRect svActionbar;
    [Inject] private ScrollRect svPoint;
    [Inject] private ScrollRect svWaitQueueArea;

    [Inject] private TextMeshProUGUI txtCount;
    [Inject] private TextMeshProUGUI txtDmg;
    [Inject] private TextMeshProUGUI txtActingTip;
    [Inject] private TextMeshProUGUI txtUltimateTip;

    [Inject] private Image imgActingIcon;
    [Inject] private Image imgIcon;

    // 行动提示对象
    private ActingTipUI actingTipUI;

    /// <summary>
    /// 操作区域
    /// </summary>
    [Inject] public RectTransform OperatorArea { get; private set; }

    /// <summary>
    /// 玩家状态区域
    /// </summary>
    [Inject] public RectTransform PlayerArea { get; private set; }

    /// <summary>
    /// 目标标记区域
    /// </summary>
    [Inject] public RectTransform SelectMarkerArea { get; private set; }

    /// <summary>
    /// 怪物状态区域
    /// </summary>
    [Inject] public RectTransform MonsterStateArea { get; private set; }

    /// <summary>
    /// 战斗结束区域
    /// </summary>
    [Inject] public RectTransform BattleOverArea { get; private set; }

    /// <summary>
    /// 状态文本区域
    /// </summary>
    [Inject] public RectTransform BuffTextArea { get; private set; }

    /// <summary>
    /// 战斗消息区域
    /// </summary>
    [Inject] public RectTransform BattleMsgArea { get; private set; }

    /// <summary>
    /// 总伤害区域
    /// </summary>
    [Inject] public RectTransform TotalDmgArea { get; private set; }

    /// <summary>
    /// 立绘显示区域
    /// </summary>
    [Inject] public RectTransform PaintingDisplayArea { get; private set; }

    /// <summary>
    /// 技能键组
    /// </summary>
    [Inject] public ToggleGroup SkillKeyGroup { get; private set; }

    /// <summary>
    /// 行动栏内容
    /// </summary>
    public Transform ActionBarContent => svActionbar.content;

    /// <summary>
    /// 战机点内容
    /// </summary>
    public Transform PointContent => svPoint.content;

    /// <summary>
    /// 等待队列内容
    /// </summary>
    public Transform WaitQueueContent => svWaitQueueArea.content;

    /// <summary>
    /// 行动提示UI对象
    /// </summary>
    public ActingTipUI ActingTipUI => actingTipUI;

    protected override void Awake()
    {
        base.Awake();

        //operatorArea = this.transform.Find(nameof(operatorArea));
        //playerArea = this.transform.Find(nameof(playerArea));
        //selectMarkerArea = this.transform.Find(nameof(selectMarkerArea));
        //monsterStateArea = this.transform.Find(nameof(monsterStateArea));
        //BattleMsgArea = this.transform.Find(nameof(BattleMsgArea));
        //BuffTextArea = this.transform.Find(nameof(BuffTextArea));
        //BattleOverArea = this.transform.Find(nameof(BattleOverArea));

        BattleOverArea.gameObject.SetActive(false);
        TotalDmgArea.gameObject.SetActive(false);
        PaintingDisplayArea.gameObject.SetActive(false);

        actingTipUI = this.GetComponentInChildren<ActingTipUI>();
        actingTipUI.Init(imgActingIcon, txtActingTip);
        actingTipUI.gameObject.SetActive(false);
    }

    public override void UpdateView(string key, object value)
    {
        switch (key)
        {
            case "activeActTip":
                (bool isActive, bool isMonster) = ((bool, bool))value;
                actingTipUI.gameObject.SetActive(isActive);
                if (isActive)
                {
                    actingTipUI.UpdateTipText(isMonster);
                }
                break;
        }
    }

    /// <summary>
    /// 更新总伤害
    /// </summary>
    /// <param name="dmg"></param>
    public void UpdateTotalDmg(long dmg)
    {
        txtDmg.text = dmg.ToString();
    }

    /// <summary>
    /// 更新终结技显示
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="tip"></param>
    public void UpdateUltimateShow(bool isShow, Sprite icon, string tip)
    {
        PaintingDisplayArea.gameObject.SetActive(isShow);
        if (isShow)
        {
            imgIcon.sprite = icon;
            txtUltimateTip.text = tip;
        }
    }

    /// <summary>
    /// 更新战技点数
    /// </summary>
    /// <param name="current"></param>
    public void UpdateBattlePointCount(int current)
    {
        txtCount.text = current.ToString();
    }
}
