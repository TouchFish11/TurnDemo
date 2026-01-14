using Framework;
using Game;
using Game.Battle;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色状态UI
/// </summary>
public class RoleStateUI : BaseUIBehaviour
{
    // UI控件相关
    [Inject] private Image imgIcon;
    [Inject] private Image imgFade;
    [Inject] private Image imgHp;
    [Inject] private Image imgEnergy;
    [Inject] private Image imgShield;
    [Inject] private ScrollRect svBuffBox;
    [Inject] private TextMeshProUGUI txtBlood;

    // 血量渐变因子
    private readonly float fadeSpeed = 1f;    
    // 护盾相关
    private int currentShield;
    private int maxShield;
    // 能量相关
    private float nonFullAhpha = 0.35f;
    // 终结技技能ID
    private int ultimateSkillId;    
    // 角色相关
    private int roleId;
    // 是否触发了终结技，防止重复触发
    private bool isTriggerUltimate;
    // 战斗上下文接口
    private IBattleContext battleContext;
    // 战斗实体接口
    private IBattleEntityObject battleEntity;
    // 状态UI列表
    private readonly List<StatusGridUI> statusGridUIs = new List<StatusGridUI>();

    /// <summary>
    /// 关联角色ID
    /// </summary>
    public int RoleId => roleId;

    protected override void Awake()
    {
        base.Awake();

        battleContext = ServiceLocator.Get<IBattleManager>().GetContext();
        // 监听战斗相关事件
        battleContext.GetEventBus().AddListener<HpChangedEvent>(OnHpChanged);
        battleContext.GetEventBus().AddListener<ShieldChangedEvent>(OnShieldChanged);
        battleContext.GetEventBus().AddListener<EnergyChangedEvent>(OnEnergyChangedEvent);
        battleContext.GetEventBus().AddListener<StatusAddedEvent>(OnStatusAddedEvent);
    }

    protected override void OnEnable()
    {
        ServiceLocator.Get<IMonoManager>().AddUpdateListener(OnUpdate);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="ultimateSkillId"></param>
    public void Init(RoleProperty playerProperty, Sprite icon, int ultimateSkillId, IBattleEntityObject battleEntity)
    {
        this.battleEntity = battleEntity;
        // 记录终结技ID
        this.ultimateSkillId = ultimateSkillId;
        // 记录角色ID
        roleId = playerProperty.Id;

        RoleInfo roleInfo = BinaryDataManager.Instance.GetConfig<RoleInfoContainer>(E_ConfigLoadType.Editor).dataDic[playerProperty.Id];
        // 设置图标
        imgIcon.sprite = icon;
        PropertyComponent propertyComponent = this.battleEntity.GetComponent<PropertyComponent>();

        // 设置血量
        imgHp.fillAmount = imgFade.fillAmount = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp) / (float)propertyComponent.GetPropertyValue(E_DynamicPropertyType.MaxHp);
        txtBlood.text = $"{propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp)}/{(float)propertyComponent.GetPropertyValue(E_DynamicPropertyType.MaxHp)}";

        // 设置能量
        imgEnergy.color = roleInfo.f_elementType.ToElementTypeColor();
        int currentEnergy = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentEnergy);
        int baseEnergy = propertyComponent.GetPropertyValue(E_DynamicPropertyType.BaseEnergy);
        imgEnergy.fillAmount = currentEnergy / (float)baseEnergy;
        imgEnergy.color = new Color(imgEnergy.color.r, imgEnergy.color.g, imgEnergy.color.b, currentEnergy == baseEnergy ? 1 : nonFullAhpha);

        // 设置buff列表
        UpdateStatus();

        // 设置护盾量
        currentShield = maxShield = 0;
        UpdateShield(currentShield, maxShield);
    }

    /// <summary>
    /// 血量变化事件回调
    /// </summary>
    /// <param name="currentHp"></param>
    /// <param name="maxHp"></param>
    private void OnHpChanged(HpChangedEvent onHpChangedEvent)
    {
        if (onHpChangedEvent.Target is not PlayerObject || onHpChangedEvent.Target.BattleEntityId != roleId)
        {
            return;
        }
        imgHp.fillAmount = onHpChangedEvent.CurrentHp / (float)onHpChangedEvent.MaxHp;
        txtBlood.text = $"{onHpChangedEvent.CurrentHp}/{onHpChangedEvent.MaxHp}";
    }

    /// <summary>
    /// 能量变化事件回调
    /// </summary>
    /// <param name="energyChangedEvent"></param>
    private void OnEnergyChangedEvent(EnergyChangedEvent energyChangedEvent)
    {
        if (energyChangedEvent.Target != this.battleEntity)
        {
            return;
        }
        imgEnergy.fillAmount = energyChangedEvent.CurrentEnergy / (float)energyChangedEvent.MaxEnergy;
        imgEnergy.color = new Color(imgEnergy.color.r, imgEnergy.color.g, imgEnergy.color.b, energyChangedEvent.CurrentEnergy == energyChangedEvent.MaxEnergy ? 1 : nonFullAhpha);
        if (energyChangedEvent.CurrentEnergy == energyChangedEvent.MaxEnergy)
        {
            isTriggerUltimate = false;
        }
    }

    /// <summary>
    /// 护盾变化事件回调
    /// </summary>
    /// <param name="onShieldChangedEvent"></param>
    private void OnShieldChanged(ShieldChangedEvent onShieldChangedEvent)
    {
        if (onShieldChangedEvent.Target != this.battleEntity)
        {
            return;
        }

        UpdateShield(onShieldChangedEvent.CurrentShield, onShieldChangedEvent.ReferenceShield);
    }

    /// <summary>
    /// 更新护盾量
    /// </summary>
    /// <param name="currentShield"></param>
    /// <param name="maxShield"></param>
    private void UpdateShield(int currentShield, int maxShield)
    {
        imgShield.fillAmount = maxShield == 0 ? 0 : currentShield / (float)maxShield;
    }

    /// <summary>
    /// 新增状态事件回调
    /// </summary>
    private void OnStatusAddedEvent(StatusAddedEvent statusAddedEvent)
    {
        if (statusAddedEvent.NewStatus.Owner != this.battleEntity)
        {
            return;
        }

        IStatus status = statusAddedEvent.NewStatus;

        // 层数变化，不用处理
        switch ((E_ConflictType)status.StatusProperty.StatusInfo.f_conflictType)
        {
            case E_ConflictType.Add:
                OnConflict_Add(status);
                break;
            case E_ConflictType.Lonel:
                OnConflict_Lonel(status);
                break;
            case E_ConflictType.Cover:
                OnConflict_Cover(status);
                break;
        }
    }

    private async void OnConflict_Add(IStatus status)
    {
        // 判断是否有该ID的状态
        bool hasStatus = statusGridUIs.Any(s => s.GetStatusId() == status.StatusProperty.StatusInfo.f_id);
        if (!hasStatus)
        {
            StatusGridUI statusGridUI = await ObjectBuilder.GetObject<StatusGridUI>(E_AssetBundleType.UI, ResKeyCollection.StatusGridUI, svBuffBox.content);
            statusGridUI.Init(status);
            statusGridUIs.Add(statusGridUI);
        }
    }

    private async void OnConflict_Lonel(IStatus newStatus)
    {
        StatusGridUI statusGridUI = await ObjectBuilder.GetObject<StatusGridUI>(E_AssetBundleType.UI, ResKeyCollection.StatusGridUI, svBuffBox.content);
        statusGridUI.Init(newStatus);
        statusGridUIs.Add(statusGridUI);
    }

    private async void OnConflict_Cover(IStatus newStatus)
    {
        StatusGridUI statusGrid = statusGridUIs.FirstOrDefault(s => s.GetStatusId() == newStatus.StatusProperty.StatusInfo.f_id);
        // 放入缓存池
        PoolManager.Instance.PushObj(statusGrid.gameObject);
        // 创建新格子
        StatusGridUI statusGridUI = await ObjectBuilder.GetObject<StatusGridUI>(E_AssetBundleType.UI, ResKeyCollection.StatusGridUI, svBuffBox.content);
        statusGridUI.Init(newStatus);
        statusGridUIs.Add(statusGridUI);
    }

    /// <summary>
    /// 更新状态
    /// 回合开始更新
    /// </summary>
    public void UpdateStatus()
    {
        for (int i = statusGridUIs.Count - 1; i >= 0; i--)
        {
            if (!statusGridUIs[i].IsValid)
            {
                // 移除无效的状态
                PoolManager.Instance.PushObj(statusGridUIs[i].gameObject);
                statusGridUIs.RemoveAt(i);
            }
        }
    }

    protected override void OnButtonClick(string btnName)
    {
        switch (btnName)
        {
            case "btnSkill":
                if (!isTriggerUltimate)
                {
                    battleContext.GetEventBus().TriggerEvent(new PlayerTriggerUltimateSkillEvent(battleContext, battleEntity, ultimateSkillId));
                    isTriggerUltimate = true;
                }
                break;
        }
    }

    private void OnUpdate()
    {
        // 血量渐变逻辑
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

    protected override void OnDisable()
    {
        ServiceLocator.Get<IMonoManager>().RemoveUpdateListener(OnUpdate);
    }
}
