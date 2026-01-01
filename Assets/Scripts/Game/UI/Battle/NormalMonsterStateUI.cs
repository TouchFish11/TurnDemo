using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 普通怪物状态UI
/// </summary>
public class NormalMonsterStateUI : BaseUIBehaviour
{
    // 血量相关
    private Image imgFade;  // 渐变血条
    private Image imgHp;    // 血条
    [SerializeField] private float fadeSpeed;    // 渐变速度

    // 韧性相关
    private Image imgToughness; // 韧性条

    // 弱点相关
    private Transform weaknessBar;  // 弱点栏
    private readonly List<Image> weakneses = new List<Image>();  // 弱点图标列表

    // 战斗实体接口
    private IBattleEntityObject battleEntity;

    protected override void Awake()
    {
        base.Awake();

        imgFade = binder.GetControl<Image>(nameof(imgFade));
        imgHp = binder.GetControl<Image>(nameof(imgHp));

        imgToughness = binder.GetControl<Image>(nameof(imgToughness));

        weaknessBar = this.transform.Find(nameof(weaknessBar));

        ServiceLocator.Instance.Get<IMonoManager>().AddUpdateListener(OnUpdate);

        // 监听事件
        ServiceLocator.Instance.Get<IBattleManager>().GetContext().GetEventBus().AddListener<HpChangedEvent>(OnHpChangedEvent);
        ServiceLocator.Instance.Get<IBattleManager>().GetContext().GetEventBus().AddListener<ToughnessChangedEvent>(OnToughnessChangedEvent);
        ServiceLocator.Instance.Get<IBattleManager>().GetContext().GetEventBus().AddListener<ToughnessBrokenEvent>(OnToughnessBrokenEvent);
    }

    /// <summary>
    /// 初始化普通怪物状态UI
    /// </summary>
    /// <param name="battleEntity"></param>
    public async void Init(IBattleEntityObject battleEntity)
    {
        foreach (Image weaknessIcon in weakneses)
        {
            PoolManager.Instance.PushObj(weaknessIcon.gameObject);
        }
        weakneses.Clear();

        this.battleEntity = battleEntity;

        PropertyComponent propertyComponent = this.battleEntity.GetComponent<PropertyComponent>();
        // 初始化血量
        imgHp.fillAmount = imgFade.fillAmount = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp) / (float)propertyComponent.GetPropertyValue(E_DynamicPropertyType.MaxHp);
        imgToughness.fillAmount = 1;

        // 初始化韧性
        ToughnessComponent toughnessComponent = battleEntity.GetComponent<ToughnessComponent>();
        int currentToughnessValue = toughnessComponent.CurrentToughnessValue;
        int maxToughnessVaue = toughnessComponent.MaxToughnessVaue;
        imgToughness.fillAmount = currentToughnessValue / (float)maxToughnessVaue;

        // 初始化弱点
        foreach (E_ElementType elementType in toughnessComponent.WeakPropertys)
        {
            GameObject weaknessIconObj = await ObjectBuilder.GetOrCreateInstance(E_AssetBundleType.UI, ResKeyCollection.WeaknessUI, weaknessBar);
            Image weaknessIcon = weaknessIconObj.GetComponent<Image>();
            weaknessIcon.color = ((int)elementType).ToElementTypeColor();
            weakneses.Add(weaknessIcon);
        }
    }

    /// <summary>
    /// 血量变化事件回调
    /// </summary>
    /// <param name="hpChangedEvent"></param>
    private void OnHpChangedEvent(HpChangedEvent hpChangedEvent)
    {
        if (hpChangedEvent.Target != battleEntity)
        {
            return;
        }

        // 更新当前血量UI
        imgHp.fillAmount = hpChangedEvent.CurrentHp / (float)hpChangedEvent.MaxHp;
    }

    /// <summary>
    /// 韧性变化事件回调
    /// </summary>
    /// <param name="toughnessChangedEvent"></param>
    private void OnToughnessChangedEvent(ToughnessChangedEvent toughnessChangedEvent)
    {
        if (toughnessChangedEvent.Target != battleEntity)
        {
            return;
        }

        // 更新当前韧性条UI
        imgToughness.fillAmount = toughnessChangedEvent.CurrentToughness / (float)toughnessChangedEvent.MaxToughness;
    }

    /// <summary>
    /// 韧性破坏事件回调
    /// </summary>
    /// <param name="toughnessBrokenEvent"></param>
    private void OnToughnessBrokenEvent(ToughnessBrokenEvent toughnessBrokenEvent)
    {
        if (toughnessBrokenEvent.Target != battleEntity)
        {
            return;
        }

        // 韧性为0，显示破韧效果
        // ...
    }

    /// <summary>
    /// 帧更新事件回调
    /// </summary>
    private void OnUpdate()
    {
        FadeBllood();
    }


    /// <summary>
    /// 血量渐变
    /// </summary>
    private void FadeBllood()
    {
        if (imgFade.fillAmount > imgHp.fillAmount)
        {
            imgFade.fillAmount -= Time.deltaTime * fadeSpeed;
            if (imgFade.fillAmount < imgHp.fillAmount)
            {
                imgFade.fillAmount = imgHp.fillAmount;
            }
        }
    }
}
