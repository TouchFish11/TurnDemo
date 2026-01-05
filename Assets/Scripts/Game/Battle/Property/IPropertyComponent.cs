using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 属性组件接口
/// </summary>
public interface IPropertyComponent
{
    T GetProperty<T>() where T : BattleProperty;
    int GetPropertyBonus(E_PropertyBonusType bonusType);
    int GetPropertyValue(E_DynamicPropertyType dynamicPropertyType);
    void SetPropertyBonus(E_PropertyBonusType bonusType, int value);
    void SetPropertyValue(E_DynamicPropertyType dynamicPropertyType, int newValue);
}
