using Framework;
using Game.Battle;
using UnityEngine;

/// <summary>
/// 目标标记对象
/// </summary>
public class TargetMarker : BaseUIBehaviour
{
    //自身变换
    private RectTransform _rectTransform;
    //目标变换
    private Transform _targetTransform;
    //主摄像机
    private Camera _mainCamera;
    //UI摄像机
    private Camera _UICamera;
    //标记偏移
    [SerializeField]
    private Vector2 markerOffset;
    //标记旋转速度
    [SerializeField]
    private float markerRotationSpeed;
    //标记脉冲速度
    [SerializeField]
    private float markerPulseSpeed;
    //标记脉冲缩放
    [SerializeField]
    private float markerPulseScale;

    protected override void Awake()
    {
        _rectTransform = this.GetComponent<RectTransform>();
        _UICamera = UIManager.Instance.UICamera;
        _mainCamera = Camera.main;
    }

    /// <summary>
    /// 初始化标记
    /// </summary>
    /// <param name="target"></param>
    public void Init(IBattleEntityObject target)
    {
        // 记录目标位置
        this._targetTransform = target.GameObject.transform;
        // 设置父对象
        // _rectTransform.SetParent(_battlePanel.GetSelectTargetUIParent(), false);
        // 更新行动轴UI选择目标显示
        // EventCenter.Instance.TriggerEvent(E_EventType.OnSelectTarget);
    }

    private void Update()
    {
        if (_targetTransform == null || _mainCamera == null || _UICamera == null)
        {
            return;
        }

        // 将敌人的世界坐标转换为屏幕坐标
        Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(_mainCamera, _targetTransform.position + (Vector3)markerOffset);

        //屏幕坐标转UI相对坐标
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(/*_battlePanel.GetSelectTargetUIParent()*/ (UIManager.Instance.Canvas.transform as RectTransform), screenPosition, _UICamera, out Vector2 localPoint))
        {
            // 设置标记位置
            _rectTransform.anchoredPosition = localPoint;
        }
        else
        {
            LogManager.LogError($"屏幕转UI坐标转换失败：目标标记：{this.gameObject}");
        }

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
}
