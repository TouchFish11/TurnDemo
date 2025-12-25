using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 选择标记UI
/// </summary>
public class SelectMarkerUI : BaseUIBehaviour
{
    // 图像UI列表
    private readonly List<Image> images = new List<Image>();
    //标记偏移
    [SerializeField] private Vector2 markerOffset;
    //标记旋转速度
    [SerializeField] private float markerRotationSpeed;
    //标记脉冲速度
    [SerializeField] private float markerPulseSpeed;
    //标记脉冲缩放
    [SerializeField] private float markerPulseScale;

    // 起始旋转
    private Quaternion originQuaterion;
    private Vector3 originScale;

    // 起始缩放

    // 标记颜色相关
    private Color enermyRed = Color.red;
    private Color friendBlue = Color.blue;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < 5; i++)
        {
            images.Add(binder.GetControl<Image>($"m{i + 1}"));
        }

        originQuaterion = this.transform.rotation;
        originScale = this.transform.localScale;
    }

    protected override void OnEnable()
    {
        this.transform.rotation = originQuaterion;
        this.transform.localScale = originScale;
        MonoManager.Instance.AddUpdateListener(OnUpdate);
    }

    /// <summary>
    /// 初始化选择标记
    /// </summary>
    /// <param name="skillTargetType"></param>
    public void InitSelectMarker(E_SkillTargetType skillTargetType)
    {
        Color color = skillTargetType == E_SkillTargetType.Enemy ? enermyRed : friendBlue;
        foreach (Image image in images)
        {
            image.color = color;
        }
    }

    private void OnUpdate()
    {
        //更新标记动画
        UpdateMarkerAnimation();
    }

    /// <summary>
    /// 更新标记动画
    /// </summary>
    private void UpdateMarkerAnimation()
    {
        // 旋转动画
        this.transform.Rotate(Vector3.forward, markerRotationSpeed * Time.deltaTime);
        // 脉冲缩放动画
        float scale = 1f + Mathf.Sin(Time.time * markerPulseSpeed) * (markerPulseScale - 1f) * 0.5f;
        this.transform.localScale = Vector3.one * scale;
    }

    protected override void OnDisable()
    {
        if (MonoManager.IsLIve)
        {
            MonoManager.Instance.RemoveUpdateListener(OnUpdate);
        }
    }
}
