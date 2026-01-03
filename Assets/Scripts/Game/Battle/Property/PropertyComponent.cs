using Framework;
using Game.Battle;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 属性组件
/// 管理战斗实体的各种属性
/// </summary>
public abstract class PropertyComponent : BattleComponent
{
    // 属性类型到加成数值（百分比）映射
    private readonly Dictionary<E_PropertyBonusType, int> _bonusToValueMap = new Dictionary<E_PropertyBonusType, int>();
    // 战斗属性
    protected BattleProperty battleProperty;
    // 战斗上下文
    protected IBattleContext battleContext;

    public bool IsDeath { get; protected set; }

    public override void BattleInit(IBattleEntityObject battleEntity)
    {
        base.BattleInit(battleEntity);
        battleContext = battleEntity.Context;
    }

    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="dynamicPropertyType"></param>
    /// <param name="damageType"></param>
    /// <param name="newValue"></param>
    public virtual void SetPropertyValue(E_DynamicPropertyType dynamicPropertyType, int newValue)
    {
        switch (dynamicPropertyType)
        {
            case E_DynamicPropertyType.CurrentHp:
                int currentHpDelta = battleProperty.CurrentHp - newValue;
                battleProperty.CurrentHp = newValue;
                battleContext.GetEventBus().TriggerEvent(new HpChangedEvent(battleContext, newValue, battleProperty.MaxHp, currentHpDelta, BattleEntity));
                break;
            case E_DynamicPropertyType.MaxHp:
                int maxHpDelta = battleProperty.MaxHp - newValue;
                battleProperty.MaxHp = newValue;
                battleContext.GetEventBus().TriggerEvent(new HpChangedEvent(battleContext, battleProperty.CurrentHp, newValue, maxHpDelta, BattleEntity));
                break;
            case E_DynamicPropertyType.TotalAtk:
                battleProperty.TotalAtk = newValue;

                break;
            case E_DynamicPropertyType.TotalDef:
                battleProperty.TotalDef = newValue;

                break;
            case E_DynamicPropertyType.CurrentSpeed:
                battleProperty.CurrentSpeed = newValue;

                break;
            case E_DynamicPropertyType.TotalCrit:
                battleProperty.TotalCrit = newValue;
                break;
            case E_DynamicPropertyType.TotalCritDmg:
                battleProperty.TotalCritDmg = newValue;
                break;
        }
    }

    /// <summary>
    /// 获取属性值
    /// </summary>
    /// <param name="dynamicPropertyType"></param>
    /// <returns></returns>
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
            default:
                LogManager.LogError($"未找到动态属性类型，{dynamicPropertyType}，已返回默认值{default}");
                return default;
        }
    }

    /// <summary>
    /// 获取属性
    /// </summary>
    /// <returns></returns>
    public T GetProperty<T>() where T : BattleProperty
    {
        return battleProperty as T;
    }

    /// <summary>
    /// 设置属性加成
    /// </summary>
    /// <param name="bonusType"></param>
    /// <param name="value"></param>
    public void SetPropertyBonus(E_PropertyBonusType bonusType, int value)
    {
        if (_bonusToValueMap.ContainsKey(bonusType))
        {
            _bonusToValueMap[bonusType] += value;
        }
        else
        {
            _bonusToValueMap.Add(bonusType, value);
        }
    }

    /// <summary>
    /// 获取属性加成
    /// </summary>
    /// <param name="bonusType"></param>
    /// <returns></returns>
    public int GetPropertyBonus(E_PropertyBonusType bonusType)
    {
        if (_bonusToValueMap.TryGetValue(bonusType, out var value))
        {
            return value;
        }

        LogManager.LogWarning($"该属性加成不存在，{bonusType}，已返回{default}");
        return default;
    }
}
