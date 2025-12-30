using Framework;
using Game.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 行动格子UI
/// </summary>
public class ActionGridUI : BaseUIBehaviour
{
    private Image imgSelect;
    private Image imgIcon;
    private TextMeshProUGUI txtActionValue;
    private GameObject flashing;

    //图片数组
    private Image[] images;
    // 原始颜色
    private Color white = Color.white;
    // 透明
    private Color transparent = new Color(0, 0, 0, 0);

    private float currentAlpha = 1f;


    // 图片选择起始位置
    private Vector3 imgSelectOriginPos;
    // 战斗实体ID
    private int battleEntityId;
    // 是否选中
    private bool isSelect;

    // 移动幅度
    [SerializeField] private float moveRange;
    // 移动速度
    [SerializeField] private float moveSpeed;


    protected override void Awake()
    {
        base.Awake();
        imgSelect = binder.GetControl<Image>(nameof(imgSelect));
        imgSelectOriginPos = imgSelect.transform.localPosition;
        imgSelect.gameObject.SetActive(false);

        imgIcon = binder.GetControl<Image>(nameof(imgIcon));
        txtActionValue = binder.GetControl<TextMeshProUGUI>(nameof(txtActionValue));
        flashing = this.transform.Find(nameof(flashing)).gameObject;
        images = flashing.GetComponentsInChildren<Image>();
        flashing.SetActive(false);

        ServiceLocator.Instance.Get<IMonoManager>().AddUpdateListener(OnUpdate);
    }

    /// <summary>
    /// 初始化UI
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="actionValue"></param>
    public void Init(Sprite icon, float actionValue, int battleEntityId)
    {
        this.battleEntityId = battleEntityId;
        imgSelect.transform.localPosition = imgSelectOriginPos;
        imgIcon.sprite = icon;
        txtActionValue.text = ((int)actionValue).ToString();
    }

    /// <summary>
    /// 检查选中状态
    /// </summary>
    /// <param name="entityId"></param>
    public void CheckSelect(int entityId)
    {
        isSelect = entityId == battleEntityId;
        // 闪烁动画
        SetFlashing();
        // 选择动画
        SetSelecting();
    }

    private void SetFlashing()
    {
        flashing.SetActive(isSelect);
    }

    private void SetSelecting()
    {
        imgSelect.gameObject.SetActive(isSelect);
    }

    private void OnUpdate()
    {
        if (!isSelect)
        {
            return;
        }

        // 选中动画
        imgSelect.transform.localPosition = Mathf.Sin(Time.time * moveSpeed) * moveRange * Vector3.right;


        // 闪烁动画
        if (currentAlpha > 0)
        {
            currentAlpha -= Time.deltaTime;
        }
        else if(currentAlpha < 0)
        {
            currentAlpha += Time.deltaTime;
        }

        Color color = new Color(1, 1, 1, currentAlpha);
        foreach (var image in images)
        {
            image.color = color;
        }
    }

}


