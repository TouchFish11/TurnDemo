namespace HotUpdate.Game.Battle.Property
{
    /// <summary>
    /// 属性组件接口
    /// </summary>
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
