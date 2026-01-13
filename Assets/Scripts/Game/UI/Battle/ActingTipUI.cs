using Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 行动提示UI
/// </summary>
public class ActingTipUI : UIBehaviour
{
    private Image imgActingIcon;
    private TextMeshProUGUI txtActingTip;

    private const string PlayerTipText = "我方行动中...";
    private const string MonsterTipText = "敌方行动中...";

    [SerializeField] private float moveRange = 6f;
    [SerializeField] private float moveSpeed = 7f;

    // 起始位置
    private Vector3 originTrans;

    protected override void OnEnable()
    {
        imgActingIcon.transform.position = originTrans;
        ServiceLocator.Get<IMonoManager>().AddUpdateListener(OnUpdate);
    }

    public void Init(Image imgActingIcon, TextMeshProUGUI txtActingTip)
    {
        originTrans = imgActingIcon.transform.position;

        this.imgActingIcon = imgActingIcon;
        this.txtActingTip = txtActingTip;
    }

    public void UpdateTipText(bool isMonster)
    {
        txtActingTip.text = isMonster ? MonsterTipText : PlayerTipText;
    }

    private void OnUpdate()
    {
        // 图标动画
        imgActingIcon.transform.localPosition = Mathf.Sin(Time.time * moveSpeed) * moveRange * imgActingIcon.transform.right;

        // 文本动画
    }

    protected override void OnDisable()
    {
        ServiceLocator.Get<IMonoManager>().RemoveUpdateListener(OnUpdate);
    }
}
