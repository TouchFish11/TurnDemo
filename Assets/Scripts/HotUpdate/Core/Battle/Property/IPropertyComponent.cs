using HotUpdate.Battle.Property;

namespace HotUpdate.Core.Battle.Property
{
    public interface IPropertyComponent
    {
        bool IsDeath { get; }

        void SetPropertyValue(E_DynamicPropertyType dynamicPropertyType, int newValue);

        int GetPropertyValue(E_DynamicPropertyType dynamicPropertyType);

        T GetProperty<T>() where T : BattleProperty;

        void SetPropertyBonus(E_PropertyBonusType bonusType, int value);
        
        int GetPropertyBonus(E_PropertyBonusType bonusType);
    }
}
