using Framework;
using Game.Battle;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 普通怪物状态UI
/// </summary>
public class NormalMonsterStateUI : BaseUIBehaviour
{
    // 渐变血条
    [Inject] private Image imgFade;
    // 血条                                
    [Inject] private Image imgHp;
    // 韧性条
    [Inject] private Image imgToughness;

    /// <summary>
    /// 弱点栏
    /// </summary>
    [Inject(1)] private RectTransform WeaknessBar { get; set; }

    // 渐变速度
    [SerializeField] private float fadeSpeed;

    // 弱点图标列表
    private readonly List<Image> weakneses = new List<Image>();  
    // 战斗实体接口
    private IBattleEntityObject battleEntity;
    // UI父对象
    private Transform monsterStateArea;
    // 上一次的位置
    private Vector3 lastPos;

    public IBattleEntityObject BattleEntity => battleEntity;

    protected override void Awake()
    {
        base.Awake();

        // 监听事件
        ServiceLocator.Get<IBattleManager>().GetContext().GetEventBus().AddListener<HpChangedEvent>(OnHpChangedEvent);
        ServiceLocator.Get<IBattleManager>().GetContext().GetEventBus().AddListener<ToughnessChangedEvent>(OnToughnessChangedEvent);
        ServiceLocator.Get<IBattleManager>().GetContext().GetEventBus().AddListener<ToughnessBrokenEvent>(OnToughnessBrokenEvent);
    }

    protected override void OnEnable()
    {
        ServiceLocator.Get<IMonoManager>().AddUpdateListener(OnUpdate);
    }

    /// <summary>
    /// 初始化普通怪物状态UI
    /// </summary>
    /// <param name="battleEntity"></param>
    public async void Init(IBattleEntityObject battleEntity, Transform monsterStateArea)
    {
        foreach (Image weaknessIcon in weakneses)
        {
            PoolManager.Instance.PushObj(weaknessIcon.gameObject);
        }
        weakneses.Clear();

        this.battleEntity = battleEntity;
        this.monsterStateArea = monsterStateArea;

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
            GameObject weaknessIconObj = await ObjectBuilder.GetOrCreateInstance(E_AssetBundleType.UI, ResKeyCollection.WeaknessUI, WeaknessBar);
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
        FollowTarget();
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

        UIManager.WorldToLocalPointInRectangle(BattlePoint.Instance.CurrentActiveCamera, UIManager.Instance.UICamera, monsterStateArea, this.gameObject, battleEntity.GameObject.transform.position, Vector2.up * 250);
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

    protected override void OnDisable()
    {
        ServiceLocator.Get<IMonoManager>().RemoveUpdateListener(OnUpdate);
    }
}
