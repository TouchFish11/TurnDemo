using Game.Battle;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 属性组件
/// 管理战斗实体的各种属性
/// </summary>
public abstract class PropertyComponent : BattleComponent
{
    // 额外属性加成
    private readonly Dictionary<E_DynamicPropertyType, int> _propertyBonuses = new Dictionary<E_DynamicPropertyType, int>();
    // 战斗属性
    protected BattleProperty battleProperty;

    public bool IsDeath { get; protected set; }

    public void SetProperty(E_DynamicPropertyType dynamicPropertyType, int newValue)
    {
        IBattleContext battleContext = BattleManager.Instance.GetContext();
        switch (dynamicPropertyType)
        {
            case E_DynamicPropertyType.CurrentHp:
                battleProperty.CurrentHp = newValue;
                battleContext.GetEventBus().TriggerEvent(new OnHpChangedEvent(battleContext, newValue, battleProperty.MaxHp));
                break;
            case E_DynamicPropertyType.MaxHp:
                battleProperty.MaxHp = newValue;
                battleContext.GetEventBus().TriggerEvent(new OnHpChangedEvent(battleContext, battleProperty.CurrentHp, newValue));
                break;
            case E_DynamicPropertyType.MaxAtk:
                battleProperty.MaxAtk = newValue;

                break;
            case E_DynamicPropertyType.MaxDef:
                battleProperty.MaxDef = newValue;

                break;
            case E_DynamicPropertyType.CurrentSpeed:
                battleProperty.CurrentSpeed = newValue;

                break;
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

    public virtual void AddBonus(E_RelicBoun type, int value)
    {
        //E_DynamicPropertyType fieldType = E_DynamicPropertyType.None;
        //switch (type)
        //{
        //    case E_RelicBoun.CriticalRate:
        //        fieldType = E_DynamicPropertyType.CriticalRate;
        //        break;
        //    case E_RelicBoun.CriticalDmg:
        //        fieldType = E_DynamicPropertyType.CriticalDmg;
        //        break;
        //    case E_RelicBoun.BuildHp:
        //        fieldType = E_DynamicPropertyType.BaseHp;
        //        break;
        //    case E_RelicBoun.Speed:
        //        fieldType = E_DynamicPropertyType.BaseSpeed;
        //        break;
        //}

        //if (_propertyBonuses.ContainsKey(fieldType))
        //{
        //    _propertyBonuses[fieldType] += value;
        //}
        //else
        //{
        //    _propertyBonuses[fieldType] = value;
        //}
    }

}
