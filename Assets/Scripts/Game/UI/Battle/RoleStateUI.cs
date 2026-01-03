using Framework;
using Game;
using Game.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色状态UI
/// </summary>
public class RoleStateUI : BaseUIBehaviour
{
    // UI控件相关
    private Image imgIcon;
    private Image imgFade;
    private Image imgHp;
    private Image imgEnergy;
    private Image imgShield;
    private ScrollRect svBuffBox;
    private TextMeshProUGUI txtBlood;

    private readonly float fadeSpeed = 1f;    // 血量渐变因子

    // 护盾相关
    private int currentShield;
    private int maxShield;

    // 技能相关
    private int ultimateSkillId;    // 终结技技能ID

    // 角色相关
    private int roleId;

    // 战斗上下文接口
    private IBattleContext battleContext;
    // 战斗实体接口
    private IBattleEntityObject battleEntity;

    protected override void Awake()
    {
        base.Awake();

        imgIcon = binder.GetControl<Image>(nameof(imgIcon));
        imgFade = binder.GetControl<Image>(nameof(imgFade));
        imgHp = binder.GetControl<Image>(nameof(imgHp));
        imgEnergy = binder.GetControl<Image>(nameof(imgEnergy));
        imgShield = binder.GetControl<Image>(nameof(imgShield));
        svBuffBox = binder.GetControl<ScrollRect>(nameof(svBuffBox));
        txtBlood = binder.GetControl<TextMeshProUGUI>(nameof(txtBlood));

        // 监听战斗相关事件

        // 应该是数据驱动
        battleContext = ServiceLocator.Instance.Get<IBattleManager>().GetContext();

        battleContext.GetEventBus().AddListener<HpChangedEvent>(OnHpChanged);
        battleContext.GetEventBus().AddListener<EnergyChangedEvent>(OnEnergyChangedEvent);

        ServiceLocator.Instance.Get<IMonoManager>().AddUpdateListener(OnUpdate);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="ultimateSkillId"></param>
    public async void Init(RoleProperty playerProperty, int ultimateSkillId, IBattleEntityObject battleEntity)
    {
        this.battleEntity = battleEntity;
        // 记录终结技ID
        this.ultimateSkillId = ultimateSkillId;
        // 记录角色ID
        roleId = playerProperty.Id;

        RoleInfo roleInfo = BinaryDataManager.Instance.GetConfig<RoleInfoContainer>(E_ConfigLoadType.Editor).dataDic[playerProperty.Id];
        // 设置图标
        // imgIcon.sprite = await AssetBundleManager.Instance.LoadAssetAsync<Sprite>(E_AssetBundleType.Texture, ResKeyCollection.WhiteImage);
        PropertyComponent propertyComponent = this.battleEntity.GetComponent<PropertyComponent>();

        // 设置血量
        imgHp.fillAmount = imgFade.fillAmount = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp) / (float)propertyComponent.GetPropertyValue(E_DynamicPropertyType.MaxHp);
        txtBlood.text = $"{propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp)}/{(float)propertyComponent.GetPropertyValue(E_DynamicPropertyType.MaxHp)}";

        // 设置能量
        imgEnergy.color = roleInfo.f_elementType.ToElementTypeColor();
        imgEnergy.fillAmount = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentEnergy) / (float)propertyComponent.GetPropertyValue(E_DynamicPropertyType.BaseEnergy);

        // 设置buff列表
        UpdateBuff();

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
    }

    /// <summary>
    /// 护盾变化事件回调
    /// </summary>
    /// <param name="onShieldChangedEvent"></param>
    private void OnShieldChanged(ShieldChangedEvent onShieldChangedEvent)
    {
        UpdateShield(onShieldChangedEvent.CurrentShield, onShieldChangedEvent.MaxShield);
    }

    /// <summary>
    /// 更新护盾量
    /// </summary>
    /// <param name="currentShield"></param>
    /// <param name="maxShield"></param>
    public void UpdateShield(int currentShield, int maxShield)
    {
        imgShield.fillAmount = maxShield == 0 ? 0 : currentShield / (float)maxShield;
    }

    /// <summary>
    /// 更新Buff
    /// </summary>
    public void UpdateBuff()
    {

    }

    protected override void OnButtonClick(string btnName)
    {
        switch (btnName)
        {
            case "btnSkill":
                battleContext.GetEventBus().TriggerEvent(new PlayerTriggerUltimateSkillEvent(battleContext, battleEntity, ultimateSkillId));
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

}
