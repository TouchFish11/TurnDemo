using Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战斗消息UI
/// </summary>
public class BattleMessageUI : BaseUIBehaviour
{
    [Inject] private TextMeshProUGUI txtMsg;
    [Inject] private Image msg;
    [Inject] private Image imgIcon;

    // 透明度
    private float msgAlpha;
    private float imgIconAlpha;

    [SerializeField] private float duration;
    // 当前持续时间
    private float currentDuration;

    protected override void Awake()
    {
        base.Awake();

        msgAlpha = msg.color.a;
        imgIconAlpha = imgIcon.color.a;
    }

    protected override void OnEnable()
    {
        currentDuration = 0;
        ServiceLocator.Get<IMonoManager>().AddUpdateListener(OnUpdate);
    }

    public void InitMessage(Color color, string msg)
    {
        this.msg.color = new Color(color.r, color.g, color.b, msgAlpha);
        this.imgIcon.color = new Color(color.r, color.g, color.b, imgIconAlpha);
        txtMsg.text = msg;
    }

    private void OnUpdate()
    {
        currentDuration += Time.deltaTime;
        if (currentDuration >= duration)
        {
            PoolManager.Instance.PushObj(this.gameObject);
        }
    }

    protected override void OnDisable()
    {
        ServiceLocator.Get<IMonoManager>().RemoveUpdateListener(OnUpdate);
    }
}
