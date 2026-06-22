using System.Collections.Generic;
using Core.Log;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Event.General;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Property
{
    /// <summary>
    /// 属性组件抽象类
    /// 封装战斗实体的核心属性管理逻辑，包括属性读写、加成配置、属性变更事件触发等
    /// 所有具体的战斗属性组件需继承此类实现
    /// </summary>
    public abstract class PropertyComponent : BattleComponent, IPropertyComponent
    {
        // 存储属性加成类型与对应数值的映射字典（key：加成类型，value：加成数值）
        private readonly Dictionary<E_PropertyBonusType, int> _bonusToValueMap = new();
        // 当前战斗实体的核心属性容器
        protected BattleProperty battleProperty;
        // 战斗上下文（用于获取事件总线、战斗环境信息等）
        protected IBattleContext battleContext;

        /// <summary>
        /// 战斗实体是否死亡标识
        /// </summary>
        public bool IsDeath { get; protected set; }
        
        public void InitProperty(IBattleEntityObject battleEntity)
        {
            BattleInit(battleEntity);
            // 从战斗实体中获取战斗上下文
            battleContext = BattleEntity.Context;
            OnInitProperty();
        }

        /// <summary>
        /// 初始化属性
        /// </summary>
        protected abstract void OnInitProperty();
        
        /// <summary>
        /// 设置动态属性值（会触发对应属性变更事件）
        /// </summary>
        /// <param name="dynamicPropertyType">动态属性类型（如当前血量、最大血量等）</param>
        /// <param name="newValue">属性新值</param>
        public virtual void SetPropertyValue(E_DynamicPropertyType dynamicPropertyType, int newValue)
        {
            switch (dynamicPropertyType)
            {
                case E_DynamicPropertyType.CurrentHp:
                    // 更新当前血量
                    battleProperty.CurrentHp = newValue;
                    // 触发血量变更事件（通知事件总线）
                    battleContext.GetEventBus().TriggerEvent(new HpChangedEvent(battleContext, battleProperty.CurrentHp, battleProperty.MaxHp, BattleEntity));
                    break;
                case E_DynamicPropertyType.MaxHp:
                    // 更新最大血量
                    battleProperty.MaxHp = newValue;
                    // 触发血量变更事件（当前血量、新最大血量）
                    battleContext.GetEventBus().TriggerEvent(new HpChangedEvent(battleContext, battleProperty.CurrentHp, newValue, BattleEntity));
                    break;
                case E_DynamicPropertyType.TotalAtk:
                    // 更新总攻击力
                    battleProperty.TotalAtk = newValue;
                    break;
                case E_DynamicPropertyType.TotalDef:
                    // 更新总防御力
                    battleProperty.TotalDef = newValue;
                    break;
                case E_DynamicPropertyType.CurrentSpeed:
                    // 更新当前速度
                    battleProperty.CurrentSpeed = newValue;
                    break;
                case E_DynamicPropertyType.TotalCrit:
                    // 更新总暴击率
                    battleProperty.TotalCrit = newValue;
                    break;
                case E_DynamicPropertyType.TotalCritDmg:
                    // 更新总暴击伤害
                    battleProperty.TotalCritDmg = newValue;
                    break;
                case E_DynamicPropertyType.CurrentShield:
                    // 计算护盾变更差值（新值-旧值）
                    var currentShieldDelta = newValue - battleProperty.CurrentShield;
                    // 更新当前护盾值
                    battleProperty.CurrentShield = newValue;
                    // 触发护盾变更事件
                    battleContext.GetEventBus().TriggerEvent(new ShieldChangedEvent(battleContext, battleProperty.CurrentShield, BattleEntity, currentShieldDelta));
                    break;
            }
        }

        /// <summary>
        /// 获取指定类型的动态属性值
        /// </summary>
        /// <param name="dynamicPropertyType">动态属性类型</param>
        /// <returns>对应属性的当前数值，未找到则返回0并打印错误日志</returns>
        public virtual int GetPropertyValue(E_DynamicPropertyType dynamicPropertyType)
        {
            switch (dynamicPropertyType)
            {
                case E_DynamicPropertyType.CurrentHp:
                    return battleProperty.CurrentHp;
                case E_DynamicPropertyType.MaxHp:
                    return battleProperty.MaxHp;
                case E_DynamicPropertyType.TotalAtk:
                    return battleProperty.TotalAtk;
                case E_DynamicPropertyType.TotalDef:
                    return battleProperty.TotalDef;
                case E_DynamicPropertyType.CurrentSpeed:
                    return battleProperty.CurrentSpeed;
                case E_DynamicPropertyType.TotalCrit:
                    return battleProperty.TotalCrit;
                case E_DynamicPropertyType.TotalCritDmg:
                    return battleProperty.TotalCritDmg;
                case E_DynamicPropertyType.CurrentShield:
                    return battleProperty.CurrentShield;
                default:
                    // 未匹配到属性类型时打印错误日志
                    Logger.LogError($"未找到动态属性类型：{dynamicPropertyType}，已返回默认值{default}");
                    return 0;
            }
        }

        /// <summary>
        /// 泛型方法：获取当前战斗属性的强类型实例
        /// </summary>
        /// <typeparam name="T">BattleProperty的子类（强类型约束）</typeparam>
        /// <returns>转换后的强类型BattleProperty实例，转换失败则返回null</returns>
        public T GetProperty<T>() where T : BattleProperty
        {
            return battleProperty as T;
        }

        /// <summary>
        /// 设置属性加成值（累加模式：存在则加，不存在则新增）
        /// </summary>
        /// <param name="bonusType">属性加成类型</param>
        /// <param name="value">要添加的加成数值（可正可负）</param>
        public void SetPropertyBonus(E_PropertyBonusType bonusType, int value)
        {
            // 尝试添加新的加成类型&数值，添加失败则累加已有数值
            if (!_bonusToValueMap.TryAdd(bonusType, value))
            {
                _bonusToValueMap[bonusType] += value;
            }
        }

        /// <summary>
        /// 获取指定类型的属性加成值
        /// </summary>
        /// <param name="bonusType">属性加成类型</param>
        /// <returns>对应加成数值，无则返回0并打印警告日志</returns>
        public int GetPropertyBonus(E_PropertyBonusType bonusType)
        {
            // 尝试从字典中获取加成值
            if (_bonusToValueMap.TryGetValue(bonusType, out var value))
            {
                return value;
            }

            // 未找到加成类型时打印警告日志
            Logger.LogWarning($"属性加成映射不存在：{bonusType}，已返回{default}");
            return 0;
        }
    }
}