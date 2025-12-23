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

    // 血量相关
    private int maxHp;
    private int currentHp;
    private float currentFadeHp;
    private readonly float fadeFactor = 2f;    // 血量渐变因子

    // 能量相关
    private int currentEnergy;
    private int maxEnergy;

    // 护盾相关
    private int currentShield;
    private int maxShield;

    // 技能相关
    private int ultimateSkillId;    // 终结技技能ID

    // 角色相关
    private int roleId;

    /// <summary>
    /// 终结技触发事件
    /// </summary>
    public event Action<int> OnTriggerUltimateSkill;

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
        BattleManager.Instance.GetContext().GetEventBus().AddListener<OnHpChangedEvent>(OnHpChanged); 

        MonoManager.Instance.AddUpdateListener(OnUpdate);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="ultimateSkillId"></param>
    public async void Init(RoleProperty playerProperty, int ultimateSkillId)
    {
        RoleInfo roleInfo = BinaryDataMgr.Instance.GetConfig<RoleInfoContainer>(E_ConfigLoadType.Editor).dataDic[playerProperty.Id];
        // 设置图标
        // imgIcon.sprite = await AssetBundleManager.Instance.LoadAssetAsync<Sprite>(E_AssetBundleType.Texture, ResKeyCollection.WhiteImage);
        // 设置血量
        maxHp = currentHp = (int)(currentFadeHp = playerProperty.BaseHp);
        UpdateHp(currentHp, maxHp);
        // 设置能量
        maxEnergy = playerProperty.BaseEnergy;
        currentEnergy = 0;  // 暂时默认为0
        imgEnergy.color = roleInfo.f_elementType.ToElementTypeColor();
        UpdateEnergy(currentEnergy, maxEnergy);
        // 设置buff列表
        UpdateBuff();
        // 设置护盾量
        currentShield = maxShield = 0;
        UpdateShield(currentShield, maxShield);
        // 记录终结技ID
        this.ultimateSkillId = ultimateSkillId;
        // 记录角色ID
        roleId = playerProperty.Id;
    }

    /// <summary>
    /// 血量变化事件回调
    /// </summary>
    /// <param name="currentHp"></param>
    /// <param name="maxHp"></param>
    private void OnHpChanged(OnHpChangedEvent onHpChangedEvent)
    {
        if (onHpChangedEvent.Target is not PlayerObject || onHpChangedEvent.Target.BattleEntityId != roleId)
        {
            return;
        }

        LogManager.Log($"受伤的实体ID：{onHpChangedEvent.Target.BattleEntityId}，角色ID：{roleId}");

        UpdateHp(onHpChangedEvent.CurrentHp, onHpChangedEvent.MaxHp);
    }

    /// <summary>
    /// 护盾变化事件回调
    /// </summary>
    /// <param name="onShieldChangedEvent"></param>
    private void OnShieldChanged(OnShieldChangedEvent onShieldChangedEvent)
    {
        UpdateShield(onShieldChangedEvent.CurrentShield, onShieldChangedEvent.MaxShield);
    }

    /// <summary>
    /// 更新血量
    /// </summary>
    /// <param name="currentHp"></param>
    /// <param name="maxHp"></param>
    private void UpdateHp(int currentHp, int maxHp)
    {
        this.currentHp = currentHp;
        this.maxHp = maxHp;
        imgHp.fillAmount = currentHp / maxHp;
        txtBlood.text = $"{currentHp}/{maxHp}";
    }

    /// <summary>
    /// 更新能量
    /// </summary>
    /// <param name="currentEnergy"></param>
    /// <param name="maxEnergy"></param>
    public void UpdateEnergy(int currentEnergy, int maxEnergy)
    {
        imgEnergy.fillAmount = currentEnergy / (float)maxEnergy;
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
                // BattleManager.Instance.GetContext().GetEventBus().TriggerEvent(new TriggerUltimateSkillEvent(BattleManager.Instance.GetContext()) { UltimateSkillId = ultimateSkillId });
                OnTriggerUltimateSkill?.Invoke(ultimateSkillId);
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
        if(currentFadeHp > currentHp)
        {
            currentFadeHp -= fadeFactor * Time.deltaTime;
            if(currentFadeHp < currentHp)
            {
                currentFadeHp = currentHp;
            }
            imgFade.fillAmount = currentFadeHp / maxHp;
        }
    }

}
