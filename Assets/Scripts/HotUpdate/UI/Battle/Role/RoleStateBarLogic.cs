using System.Linq;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Mono;
using Core.Pool;
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
using UnityEngine;

namespace HotUpdate.UI.Battle.Role
{
    public class RoleStateBarLogic : IUILogic<RoleStateBar, RoleStateBarLogic>, IPoolData
    {
        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private IMonoAdapter _monoAdapter;
        [Inject] private IPoolManager _poolManager;
        
        // 终极技能ID
        private int ultimateSkillId;    
        // 角色相关
        private IBattleContext battleContext;  // 战斗上下文接口
        private IBattleEntityObject battleEntity;  // 战斗实体对象
        // 护盾相关变量
        public int currentShield;  // 当前护盾值
        
        public RoleStateBar View { get; private set; }
        
        /// <summary>
        /// 当前UI绑定的角色ID
        /// </summary>
        public int RoleId { get; private set; }
        
        /// <summary>
        /// 初始化角色状态UI
        /// </summary>
        /// <param name="roleStateBar"></param>
        /// <param name="playerProperty">角色属性</param>
        /// <param name="icon">角色图标</param>
        /// <param name="ultimateSkillId">终极技能ID</param>
        /// <param name="battleEntity">战斗实体对象</param>
        public void Init(RoleStateBar roleStateBar, RoleProperty playerProperty, Sprite icon, int ultimateSkillId, IBattleEntityObject battleEntity)
        {
            View = roleStateBar;
            this.battleEntity = battleEntity;
            // 记录终极技能ID
            this.ultimateSkillId = ultimateSkillId;
            // 记录角色ID
            RoleId = playerProperty.BattleId;
            // 获取角色配置信息
            var roleInfo = DIContainer.GetInstance<IBinaryDataManager>().GetConfig<RoleInfoContainer>(EConfigLoadType.Excel).dataDic[playerProperty.BattleId];
            // 设置角色图标
            View.imgIcon.sprite = icon;
            // 获取属性组件
            var propertyComponent = this.battleEntity.GetComponent<PropertyComponent>();

            // 初始化血量显示
            View.imgHp.fillAmount = View.imgFade.fillAmount = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp) / (float)propertyComponent.GetPropertyValue(E_DynamicPropertyType.MaxHp);
            View.txtBlood.text = $"{propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp)}/{(float)propertyComponent.GetPropertyValue(E_DynamicPropertyType.MaxHp)}";

            // 初始化能量显示
            View.imgEnergy.color = roleInfo.f_elementType.ToElementTypeColor();  // 根据元素类型设置颜色
            var currentEnergy = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentEnergy);
            var baseEnergy = propertyComponent.GetPropertyValue(E_DynamicPropertyType.BaseEnergy);
            View.imgEnergy.fillAmount = currentEnergy / (float)baseEnergy;
            // 根据能量是否已满设置透明度
            View.imgEnergy.color = new Color(View.imgEnergy.color.r, View.imgEnergy.color.g, View.imgEnergy.color.b, currentEnergy == baseEnergy ? 1 : View.nonFullAhpha);
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
            View.imgHp.fillAmount = onHpChangedEvent.CurrentHp / (float)onHpChangedEvent.MaxHp;
            View.txtBlood.text = $"{onHpChangedEvent.CurrentHp}/{onHpChangedEvent.MaxHp}";
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
            View.imgEnergy.fillAmount = energyChangedEvent.CurrentEnergy / (float)energyChangedEvent.MaxEnergy;
            // 根据能量是否已满设置透明度
            View.imgEnergy.color = new Color(View.imgEnergy.color.r, View.imgEnergy.color.g, View.imgEnergy.color.b, energyChangedEvent.CurrentEnergy == energyChangedEvent.MaxEnergy ? 1 : View.nonFullAhpha);
            
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
            View.imgShield.fillAmount = currentShield / (float)referenceShield;
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
            var hasStatus = View.StatusGrids.Any(s => s.GetStatusId() == status.StatusProperty.StatusInfo.f_id);
            if (!hasStatus)
            {
                // 创建新的状态图标
                var statusGridUI = await _objectSpawner.SpawnAsync<StatusGridUI>(AssetKeys.StatusGridUI, View.svBuffBox.content);
                statusGridUI.Init(status, _monoAdapter);
                View.StatusGrids.Add(statusGridUI);
            }
        }

        /// <summary>
        /// 处理独占类型状态
        /// </summary>
        /// <param name="newStatus">新的状态</param>
        private async void OnConflict_Lonel(IStatus newStatus)
        {
            // 直接创建新的状态图标（独占类型总是创建新的）
            var statusGridUI = await _objectSpawner.SpawnAsync<StatusGridUI>(AssetKeys.StatusGridUI, View.svBuffBox.content);
            statusGridUI.Init(newStatus, _monoAdapter);
            View.StatusGrids.Add(statusGridUI);
        }

        /// <summary>
        /// 处理覆盖类型状态
        /// </summary>
        /// <param name="newStatus">新的状态</param>
        private async void OnConflict_Cover(IStatus newStatus)
        {
            // 查找已存在的相同ID状态
            var index = View.StatusGrids.FindIndex(s => s.GetStatusId() == newStatus.StatusProperty.StatusInfo.f_id);
            if (index != -1)
            {
                var statusGrid = View.StatusGrids[index];
                // 将旧状态图标回收到对象池
                _objectSpawner.Release(statusGrid);
                View.StatusGrids.RemoveAt(index);
            }
            
            // 创建新的状态图标
            var statusGridUI = await _objectSpawner.SpawnAsync<StatusGridUI>(AssetKeys.StatusGridUI, View.svBuffBox.content);
            statusGridUI.Init(newStatus, _monoAdapter);
            View.StatusGrids.Add(statusGridUI);
        }

        /// <summary>
        /// 更新状态图标列表
        /// 通常在回合开始时调用，清理已失效的状态
        /// </summary>
        public void UpdateStatus()
        {
            // 从后向前遍历，避免删除时索引问题
            for (var i = View.StatusGrids.Count - 1; i >= 0; i--)
            {
                if (View.StatusGrids[i].IsValid) 
                    continue;
                // 移除已失效的状态图标
                _objectSpawner.Release(View.StatusGrids[i]);
                View.StatusGrids.RemoveAt(i);
            }
        }

        public void TriggerUltimate()
        {
            if (!battleEntity.GetComponent<PlayerSkillComponent>().IsTrigger)
            {
                battleContext.EventBus.TriggerEvent(new RoleTriggerSkillEvent(battleContext, ultimateSkillId, battleEntity));
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
            if (View.imgFade.fillAmount > View.imgHp.fillAmount)
            {
                // 渐变减少
                View.imgFade.fillAmount -= Time.deltaTime * View.fadeSpeed;
                // 防止过度减少
                if (View.imgFade.fillAmount < View.imgHp.fillAmount)
                {
                    View.imgFade.fillAmount = View.imgHp.fillAmount;
                }
            }
        }
        
        public void Dispose()
        {
            _poolManager.PushData(this);
        }

        public void ResetData()
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
