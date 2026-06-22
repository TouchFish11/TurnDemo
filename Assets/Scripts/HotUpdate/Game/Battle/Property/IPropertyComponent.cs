using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Property
{
    /// <summary>
    /// 属性组件接口
    /// </summary>
    public interface IPropertyComponent
    {
        bool IsDeath { get; }

        /// <summary>
        /// 初始化属性组件
        /// </summary>
        /// <param name="battleEntity"></param>
        void InitProperty(IBattleEntityObject battleEntity);
        
        void SetPropertyValue(E_DynamicPropertyType dynamicPropertyType, int newValue);

        int GetPropertyValue(E_DynamicPropertyType dynamicPropertyType);

        T GetProperty<T>() where T : BattleProperty;

        void SetPropertyBonus(E_PropertyBonusType bonusType, int value);
        
        int GetPropertyBonus(E_PropertyBonusType bonusType);
    }
}
