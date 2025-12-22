using Framework;
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
    private ScrollRect svBuffBox;
    private TextMeshProUGUI txtBlood;

    // 血量相关
    private float maxHp;
    private float currentHp;
    private float currentFadeHp;
    private readonly float fadeFactor = 2f;    // 血量渐变因子

    // 能量相关
    private int currentEnergy;
    private int maxEnergy;

    // 技能相关
    private int ultimateSkillId;    // 终结技技能ID

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
        svBuffBox = binder.GetControl<ScrollRect>(nameof(svBuffBox));
        txtBlood = binder.GetControl<TextMeshProUGUI>(nameof(txtBlood));

        // 监听战斗相关事件

        // 应该是数据驱动
        BattleManager.Instance.GetContext().GetEventBus().AddListener<OnHpChangedEvent>(OnUpdateHp); 


        MonoManager.Instance.AddUpdateListener(OnUpdate);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="ultimateSkillId"></param>
    public async void Init(RoleInfo roleInfo, int ultimateSkillId)
    {
        // 设置图标
        // imgIcon.sprite = await AssetBundleManager.Instance.LoadAssetAsync<Sprite>(E_AssetBundleType.Texture, ResKeyCollection.WhiteImage);
        // 设置血量
        maxHp = currentHp = currentFadeHp = roleInfo.f_baseHp;
        // 设置能量
        maxEnergy = roleInfo.f_maxEnergy;
        imgEnergy.fillAmount = currentEnergy / (float)maxEnergy;
        // 设置buff列表

        // 记录终结技ID
        this.ultimateSkillId = ultimateSkillId;
    }

    /// <summary>
    /// 更新血量
    /// </summary>
    /// <param name="currentHp"></param>
    /// <param name="maxHp"></param>
    public void OnUpdateHp(OnHpChangedEvent onHpChangedEvent)
    {
        this.currentHp = onHpChangedEvent.CurrentHp;
        this.maxHp = onHpChangedEvent.MaxHp;
        imgHp.fillAmount = currentHp / maxHp;
        txtBlood.text = $"{currentHp}/{maxHp}";
    }

    /// <summary>
    /// 更新能量
    /// </summary>
    /// <param name="currentEnergy"></param>
    /// <param name="maxEnergy"></param>
    public void OnUpdateEnergy(int currentEnergy, int maxEnergy)
    {
        imgEnergy.fillAmount = currentEnergy / (float)maxEnergy;
    }

    // 更新护盾量


    // 更新Buff



    protected override void OnButtonClick(string btnName)
    {
        switch (btnName)
        {
            case "btnSkill":
                BattleManager.Instance.GetContext().GetEventBus().TriggerEvent(new TriggerUltimateSkillEvent(BattleManager.Instance.GetContext()) { UltimateSkillId = ultimateSkillId });
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
