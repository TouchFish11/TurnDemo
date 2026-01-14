using Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 状态效果文本
/// </summary>
public class StatusEffectTextUI : BaseUIBehaviour
{
    [Inject] private Image imgIcon;
    [Inject] private TextMeshProUGUI txtBuffName;

    [Inject(1)] private RectTransform Mover { get; set; }

    // 上移速度
    [SerializeField] private float upMoveSpeed = 2.5f;
    // 销毁时间
    [SerializeField] private float destroyTime = 0.85f;

    // 起始位置
    private Vector3 originMoverPos;
    // 当前时间
    private float currentTime;

    protected override void Awake()
    {
        base.Awake();

        originMoverPos = Mover.localPosition;
    }

    protected override void OnEnable()
    {
        Mover.localPosition = originMoverPos;
        ServiceLocator.Get<IMonoManager>().AddUpdateListener(OnUpadte);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="icon"></param>
    /// <param name="buffName"></param>
    public void InitText(Sprite icon, string buffName)
    {
        imgIcon.sprite = icon;
        txtBuffName.text = buffName;
    }

    private void OnUpadte()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= destroyTime)
        {
            currentTime = 0;
            PoolManager.Instance.PushObj(this.gameObject);
        }
        //文本运动
        this.Mover.Translate(Time.deltaTime * upMoveSpeed * Vector3.up);
    }

    protected override void OnDisable()
    {
        ServiceLocator.Get<IMonoManager>().RemoveUpdateListener(OnUpadte);
    }
}
