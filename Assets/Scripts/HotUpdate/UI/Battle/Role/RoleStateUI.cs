using System.Collections.Generic;
using System.Linq;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Mono;
using Core.Serialize.Binary;
using Core.UI;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Event.General;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Property;
using HotUpdate.Game.Battle.Skill.Component;
using HotUpdate.Game.Battle.Statuses;
using HotUpdate.Game.Battle.Utility;
using HotUpdate.UI.Battle.Status;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle.Role
{
    /// <summary>
    /// 角色状态UI组件
    /// 负责显示单个角色的血量、能量、护盾、状态图标等信息
    /// </summary>
    public class RoleStateUI : UIBehaviourBase
    {
        [Inject] private ObjectSpawner _objectSpawner;
        
        // UI控件引用
        [InjectUI] private Image imgIcon;              // 角色图标
        [InjectUI] private Image imgFade;              // 血量渐变填充条（用于血量减少时的延迟效果）
        [InjectUI] private Image imgHp;                // 当前血量填充条
        [InjectUI] private Image imgEnergy;            // 能量填充条
        [InjectUI] private Image imgShield;            // 护盾填充条
        [InjectUI] private ScrollRect svBuffBox;       // 状态图标的滚动容器
        [InjectUI] private TextMeshProUGUI txtBlood;   // 血量数值文本

        private IMonoAdapter _monoAdapter;
        // 血量渐变速度
        private const float fadeSpeed = 1f;
        // 护盾相关变量
        private int currentShield;  // 当前护盾值
        // 能量条透明度
        private const float nonFullAhpha = 0.35f;  // 能量未满时的透明度
        // 终极技能ID
        private int ultimateSkillId;    
        // 角色相关
        private IBattleContext battleContext;  // 战斗上下文接口
        private IBattleEntityObject battleEntity;  // 战斗实体对象
        // 状态UI列表
        private readonly List<StatusGridUI> statusGridUIs = new();

        /// <summary>
        /// 当前UI绑定的角色ID
        /// </summary>
        public int RoleId { get; private set; }

        /// <summary>
        /// 初始化角色状态UI
        /// </summary>
        /// <param name="playerProperty">角色属性</param>
        /// <param name="icon">角色图标</param>
        /// <param name="ultimateSkillId">终极技能ID</param>
        /// <param name="battleEntity">战斗实体对象</param>
        /// <param name="monoAdapter"></param>
        public void Init(RoleProperty playerProperty, Sprite icon, int ultimateSkillId, IBattleEntityObject battleEntity, 
            IMonoAdapter monoAdapter)
        {
            _monoAdapter = monoAdapter;
            this.battleEntity = battleEntity;
            // 记录终极技能ID
            this.ultimateSkillId = ultimateSkillId;
            // 记录角色ID
            RoleId = playerProperty.BattleId;

            // 获取角色配置信息
            var roleInfo = DIContainer.GetInstance<IBinaryDataManager>().GetConfig<RoleInfoContainer>(EConfigLoadType.Excel).dataDic[playerProperty.BattleId];
            
            // 设置角色图标
            imgIcon.sprite = icon;
            
            // 获取属性组件
            var propertyComponent = this.battleEntity.GetComponent<PropertyComponent>();

            // 初始化血量显示
            imgHp.fillAmount = imgFade.fillAmount = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp) / (float)propertyComponent.GetPropertyValue(E_DynamicPropertyType.MaxHp);
            txtBlood.text = $"{propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp)}/{(float)propertyComponent.GetPropertyValue(E_DynamicPropertyType.MaxHp)}";

            // 初始化能量显示
            imgEnergy.color = roleInfo.f_elementType.ToElementTypeColor();  // 根据元素类型设置颜色
            var currentEnergy = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentEnergy);
            var baseEnergy = propertyComponent.GetPropertyValue(E_DynamicPropertyType.BaseEnergy);
            imgEnergy.fillAmount = currentEnergy / (float)baseEnergy;
            // 根据能量是否已满设置透明度
            imgEnergy.color = new Color(imgEnergy.color.r, imgEnergy.color.g, imgEnergy.color.b, currentEnergy == baseEnergy ? 1 : nonFullAhpha);

            // 更新状态图标列表
            UpdateStatus();

            // 初始化护盾显示
            currentShield = 0;
            UpdateShield(currentShield);
            
            // 注册Update监听，用于每帧更新渐变效果
            _monoAdapter.AddUpdateListener(OnUpdate);
            
            // 获取战斗上下文并注册事件监听
            battleContext = battleEntity.Context;
            battleContext.EventBus.AddListener<HpChangedEvent>(OnHpChanged);
            battleContext.EventBus.AddListener<ShieldChangedEvent>(OnShieldChanged);
            battleContext.EventBus.AddListener<EnergyChangedEvent>(OnEnergyChangedEvent);
            battleContext.EventBus.AddListener<StatusAddedEvent>(OnStatusAddedEvent);
        }

        /// <summary>
        /// 血量变化事件回调
        /// </summary>
        /// <param name="onHpChangedEvent">血量变化事件</param>
        private void OnHpChanged(HpChangedEvent onHpChangedEvent)
        {
            // 检查事件目标是否为当前角色
            if (onHpChangedEvent.Target is not PlayerObject || onHpChangedEvent.Target.BattleEntityId != RoleId)
            {
                return;
            }
            
            // 更新血量显示
            imgHp.fillAmount = onHpChangedEvent.CurrentHp / (float)onHpChangedEvent.MaxHp;
            txtBlood.text = $"{onHpChangedEvent.CurrentHp}/{onHpChangedEvent.MaxHp}";
        }

        /// <summary>
        /// 能量变化事件回调
        /// </summary>
        /// <param name="energyChangedEvent">能量变化事件</param>
        private void OnEnergyChangedEvent(EnergyChangedEvent energyChangedEvent)
        {
            // 检查事件目标是否为当前战斗实体
            if (energyChangedEvent.Target != battleEntity)
            {
                return;
            }
            
            // 更新能量显示
            imgEnergy.fillAmount = energyChangedEvent.CurrentEnergy / (float)energyChangedEvent.MaxEnergy;
            // 根据能量是否已满设置透明度
            imgEnergy.color = new Color(imgEnergy.color.r, imgEnergy.color.g, imgEnergy.color.b, energyChangedEvent.CurrentEnergy == energyChangedEvent.MaxEnergy ? 1 : nonFullAhpha);
            
            // 能量满时重置终极技能触发标志
            if (energyChangedEvent.CurrentEnergy == energyChangedEvent.MaxEnergy)
            {
                battleEntity.GetComponent<PlayerSkillComponent>().IsTrigger = false;
            }
        }

        /// <summary>
        /// 护盾变化事件回调
        /// </summary>
        /// <param name="onShieldChangedEvent">护盾变化事件</param>
        private void OnShieldChanged(ShieldChangedEvent onShieldChangedEvent)
        {
            // 检查事件目标是否为当前战斗实体
            if (onShieldChangedEvent.Target != battleEntity)
            {
                return;
            }

            // 更新护盾显示
            UpdateShield(onShieldChangedEvent.CurrentShield);
        }

        /// <summary>
        /// 更新护盾显示
        /// </summary>
        /// <param name="currentShield">当前护盾值</param>
        private void UpdateShield(int currentShield)
        {
            // 已当前角色最大生命作为护盾的基准值
            var referenceShield = battleEntity.GetComponent<PropertyComponent>()
                .GetPropertyValue(E_DynamicPropertyType.MaxHp);
            imgShield.fillAmount = currentShield / (float)referenceShield;
        }

        /// <summary>
        /// 状态添加事件回调
        /// </summary>
        /// <param name="statusAddedEvent">状态添加事件</param>
        private void OnStatusAddedEvent(StatusAddedEvent statusAddedEvent)
        {
            // 检查状态所有者是否为当前战斗实体
            if (statusAddedEvent.NewStatus.Owner != battleEntity)
            {
                return;
            }

            var status = statusAddedEvent.NewStatus;

            // 根据状态冲突类型处理
            switch ((EConflictType)status.StatusProperty.StatusInfo.f_conflictType)
            {
                case EConflictType.Add:      // 叠加类型
                    OnConflict_Add(status);
                    break;
                case EConflictType.Lonely:   // 独占类型
                    OnConflict_Lonel(status);
                    break;
                case EConflictType.Cover:    // 覆盖类型
                    OnConflict_Cover(status);
                    break;
            }
        }

        /// <summary>
        /// 处理叠加类型状态
        /// </summary>
        /// <param name="status">要添加的状态</param>
        private async void OnConflict_Add(IStatus status)
        {
            // 判断是否已存在相同ID的状态
            var hasStatus = statusGridUIs.Any(s => s.GetStatusId() == status.StatusProperty.StatusInfo.f_id);
            if (!hasStatus)
            {
                // 创建新的状态图标
                var statusGridUI = await _objectSpawner.SpawnAsync<StatusGridUI>(AssetKeys.StatusGridUI, svBuffBox.content);
                statusGridUI.Init(status, _monoAdapter);
                statusGridUIs.Add(statusGridUI);
            }
        }

        /// <summary>
        /// 处理独占类型状态
        /// </summary>
        /// <param name="newStatus">新的状态</param>
        private async void OnConflict_Lonel(IStatus newStatus)
        {
            // 直接创建新的状态图标（独占类型总是创建新的）
            var statusGridUI = await _objectSpawner.SpawnAsync<StatusGridUI>(AssetKeys.StatusGridUI, svBuffBox.content);
            statusGridUI.Init(newStatus, _monoAdapter);
            statusGridUIs.Add(statusGridUI);
        }

        /// <summary>
        /// 处理覆盖类型状态
        /// </summary>
        /// <param name="newStatus">新的状态</param>
        private async void OnConflict_Cover(IStatus newStatus)
        {
            // 查找已存在的相同ID状态
            var index = statusGridUIs.FindIndex(s => s.GetStatusId() == newStatus.StatusProperty.StatusInfo.f_id);
            if (index != -1)
            {
                var statusGrid = statusGridUIs[index];
                // 将旧状态图标回收到对象池
                _objectSpawner.Release(statusGrid);
                statusGridUIs.RemoveAt(index);
            }
            
            // 创建新的状态图标
            var statusGridUI = await _objectSpawner.SpawnAsync<StatusGridUI>(AssetKeys.StatusGridUI, svBuffBox.content);
            statusGridUI.Init(newStatus, _monoAdapter);
            statusGridUIs.Add(statusGridUI);
        }

        /// <summary>
        /// 更新状态图标列表
        /// 通常在回合开始时调用，清理已失效的状态
        /// </summary>
        public void UpdateStatus()
        {
            // 从后向前遍历，避免删除时索引问题
            for (var i = statusGridUIs.Count - 1; i >= 0; i--)
            {
                if (statusGridUIs[i].IsValid) 
                    continue;
                // 移除已失效的状态图标
                _objectSpawner.Release(statusGridUIs[i]);
                statusGridUIs.RemoveAt(i);
            }
        }

        /// <summary>
        /// 按钮点击事件处理
        /// </summary>
        /// <param name="btnName">按钮名称</param>
        protected override void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case "btnSkill":
                    if (!battleEntity.GetComponent<PlayerSkillComponent>().IsTrigger)
                    {
                        battleContext.EventBus.TriggerEvent(new RoleTriggerSkillEvent(battleContext, ultimateSkillId, battleEntity));
                    }
                    break;
            }
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        private void OnUpdate()
        {
            // 执行血量渐变效果
            FadeBlood();
        }

        /// <summary>
        /// 血量渐变效果（延迟减少效果）
        /// </summary>
        private void FadeBlood()
        {
            if (imgFade.fillAmount > imgHp.fillAmount)
            {
                // 渐变减少
                imgFade.fillAmount -= Time.deltaTime * fadeSpeed;
                // 防止过度减少
                if (imgFade.fillAmount < imgHp.fillAmount)
                {
                    imgFade.fillAmount = imgHp.fillAmount;
                }
            }
        }

        protected override void OnDisable()
        {
            // 移除Update监听
            _monoAdapter.RemoveUpdateListener(OnUpdate);
            battleContext.EventBus.RemoveListener<HpChangedEvent>(OnHpChanged);
            battleContext.EventBus.RemoveListener<ShieldChangedEvent>(OnShieldChanged);
            battleContext.EventBus.RemoveListener<EnergyChangedEvent>(OnEnergyChangedEvent);
            battleContext.EventBus.RemoveListener<StatusAddedEvent>(OnStatusAddedEvent);
        }
    }
}