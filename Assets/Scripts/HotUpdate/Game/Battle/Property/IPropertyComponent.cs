namespace HotUpdate.Game.Battle.Property
{
    /// <summary>
    /// 属性组件接口
    /// </summary>
    public interface IPropertyComponent
    {
        void SetPropertyValue(E_DynamicPropertyType dynamicPropertyType, int newValue);
    }
}
