using Framework;
using Game.Battle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 行动格子UI
/// </summary>
public class ActionGridUI : BaseUIBehaviour
{
    [Inject] private Image imgSelect;
    [Inject] private Image imgIcon;
    [Inject] private TextMeshProUGUI txtActionValue;
    [Inject(1)] private RectTransform Flashing { get; set; }

    private RectTransform imgSelectRect;
    // 图片数组
    private Image[] images;
    // 当前透明度
    private float currentAlpha = 1f;
    // 当前计时
    private float time;
    // 缓存初始预设位置
    private Vector3 initLocalPos; 
    // 战斗实体
    private IBattleEntityObject battleEntity;
    // 是否选中
    private bool isSelect;
    // 是否是第一个格子
    private bool isFirstGrid;
    // 第一格子缩放因子
    private readonly float scaleFactor = 1.1f;
    // 行动值
    private float actionValue;

    // 移动幅度
    [SerializeField] private float moveRange;
    // 移动速度
    [SerializeField] private float moveSpeed;
    // 闪烁速度
    [SerializeField] private float falshSpeed;

    protected override void Awake()
    {
        base.Awake();

        // 缓存预制体设置的初始位置
        imgSelectRect = imgSelect.rectTransform;
        initLocalPos = imgSelectRect.localPosition;

        imgSelect.gameObject.SetActive(false);
        images = Flashing.GetComponentsInChildren<Image>();
        Flashing.gameObject.SetActive(false);
    }

    protected override void OnEnable()
    {
        ServiceLocator.Get<IMonoManager>().AddUpdateListener(OnUpdate);
    }

    /// <summary>
    /// 初始化UI
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="actionValue"></param>
    /// <param name="battleEntity"></param>
    /// <param name="isFirst"></param>
    public void Init(Sprite icon, float actionValue, IBattleEntityObject battleEntity, bool isFirst)
    {
        this.battleEntity = battleEntity;
        this.isFirstGrid = isFirst;
        imgIcon.sprite = icon;
        this.actionValue = actionValue;
        txtActionValue.text = ((int)actionValue).ToString();

        UpdateScale();
    }

    /// <summary>
    /// 更新缩放
    /// </summary>
    private void UpdateScale()
    {
        if (isFirstGrid)
        {
            this.transform.localScale = Vector3.one * scaleFactor;
        }
        else
        {
            this.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 检查选中状态
    /// </summary>
    /// <param name="entityId"></param>
    public void CheckSelect(IBattleEntityObject battleEntity)
    {
        isSelect = this.battleEntity == battleEntity;
        // 闪烁动画
        SetFlashing();
        // 选择动画
        SetSelecting();
    }

    /// <summary>
    /// 设置闪烁
    /// </summary>
    private void SetFlashing()
    {
        time = 0;
        Flashing.gameObject.SetActive(isSelect);
        foreach (var image in images)
        {
            image.color = Color.white;
        }
    }

    /// <summary>
    /// 设置选中
    /// </summary>
    private void SetSelecting()
    {
        imgSelect.gameObject.SetActive(isSelect);
        imgSelectRect.transform.localPosition = initLocalPos;
    }

    private void OnUpdate()
    {
        if (!isSelect)
        {
            return;
        }

        // 选中动画
        // 计算水平偏移（仅X轴，保留Y/Z初始值）
        float xOffset = Mathf.Sin(Time.time * moveSpeed) * moveRange;
        // 在初始位置上叠加偏移，而非覆盖
        imgSelectRect.localPosition = new Vector3(initLocalPos.x + xOffset, initLocalPos.y, initLocalPos.z);

        // 闪烁动画
        time += Time.deltaTime * falshSpeed;
        currentAlpha = 1 - Mathf.PingPong(time, 1f);

        Color color = new Color(1, 1, 1, currentAlpha);
        foreach (var image in images)
        {
            image.color = color;
        }
    }

    protected override void OnDisable()
    {
        ServiceLocator.Get<IMonoManager>().RemoveUpdateListener(OnUpdate);
    }

    /// <summary>
    /// 是否选中
    /// </summary>
    public bool IsSelect => isSelect;
}


