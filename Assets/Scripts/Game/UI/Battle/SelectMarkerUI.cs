using Framework;
using Game.Battle;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 选择标记UI
/// </summary>
public class SelectMarkerUI : BaseUIBehaviour
{
    //标记偏移
    [SerializeField] private Vector2 markerOffset;
    //标记旋转速度
    [SerializeField] private float markerRotationSpeed;
    //标记脉冲速度
    [SerializeField] private float markerPulseSpeed;
    //标记脉冲缩放
    [SerializeField] private float markerPulseScale;

    // 图像UI列表
    private readonly List<Image> images = new List<Image>();
    // 起始旋转
    private Quaternion originQuaterion;
    // 起始缩放
    private Vector3 originScale;
    // 标记颜色相关
    private Color enermyRed = Color.red;
    private Color friendBlue = Color.blue;

    // 标记目标
    private IBattleEntityObject battleEntity;
    // 标记父对象
    private Transform selectMarkerArea;

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
    public void InitSelectMarker(IBattleEntityObject battleEntity, E_SkillTargetType skillTargetType, Transform selectMarkerArea)
    {
        this.battleEntity = battleEntity;
        this.selectMarkerArea = selectMarkerArea;
        Color color = skillTargetType == E_SkillTargetType.Enemy ? enermyRed : friendBlue;
        foreach (Image image in images)
        {
            image.color = color;
        }
    }

    private void OnUpdate()
    {
        FollowTarget();
        //更新标记动画
        UpdateMarkerAnimation();
    }

    /// <summary>
    /// 跟随目标
    /// </summary>
    private void FollowTarget()
    {
        if (battleEntity == null)
        {
            return;
        }

        UIManager.WorldToLocalPointInRectangle(BattlePoint.Instance.CurrentActiveCamera, UIManager.Instance.UICamera, selectMarkerArea, this.gameObject, battleEntity.GameObject.transform.position, Vector2.up * 50);
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
        ServiceLocator.Get<IMonoManager>().RemoveUpdateListener(OnUpdate);
    }
}
