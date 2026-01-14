using Framework;
using TMPro;
using UnityEngine;

/// <summary>
/// 伤害文本UI
/// </summary>
public class DamageTextUI : BaseUIBehaviour
{
    [Inject] private TextMeshProUGUI txtDamageTip;
    [Inject] private TextMeshProUGUI txtDamageNum;

    [Inject(1)] private RectTransform DamageTextMover { get; set; }

    // 上移速度
    [SerializeField] private float upMoveSpeed = 2.5f;
    // 销毁时间
    [SerializeField] private float destroyTime = 0.85f;
    // 开始缩放
    [SerializeField] private Vector3 StartScale = Vector3.one * 1.7f;
    // 结束缩放
    [SerializeField] private Vector3 endScale = Vector3.one;
    // 缩放因子
    [SerializeField] private float scaleFactor = 9f;

    // 当前时间
    private float currentTime;
    // 原始颜色
    private Color originColor;
    // 原始透明度
    private float originAlpha;
    // 当前透明度
    private float currentAlpha;


    protected override void OnEnable()
    {
        MonoManager.Instance.AddUpdateListener(OnUpdate);
        (DamageTextMover.transform as RectTransform).anchoredPosition = Vector3.zero;
        DamageTextMover.localScale = StartScale;
        currentTime = 0;
        currentAlpha = originAlpha;
        txtDamageTip.color = originColor;
        txtDamageNum.color = originColor;
    }

    /// <summary>
    /// 初始化伤害文本
    /// </summary>
    /// <param name="textColor"></param>
    /// <param name="damageTypeText"></param>
    /// <param name="damage"></param>
    public void InitDamageText(Color textColor, string damageTypeText, int damage)
    {
        txtDamageTip.color = textColor;
        txtDamageNum.color = textColor;

        txtDamageTip.text = damageTypeText;
        txtDamageNum.text = damage.ToString();

        originColor = txtDamageTip.color;
        originAlpha = currentAlpha = txtDamageTip.color.a;
    }

    private void OnUpdate()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= destroyTime)
        {
            currentTime = 0;
            PoolManager.Instance.PushObj(this.gameObject);
        }

        // 文本缩放
        this.DamageTextMover.localScale = Vector3.Lerp(this.DamageTextMover.localScale, endScale, Time.deltaTime * scaleFactor);
        //文本运动
        this.DamageTextMover.Translate(Time.deltaTime * upMoveSpeed * Vector3.up);
    }

    protected override void OnDisable()
    {
        ServiceLocator.Get<IMonoManager>().RemoveUpdateListener(OnUpdate);
    }
}
