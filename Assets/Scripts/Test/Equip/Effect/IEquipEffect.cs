using HotUpdate.Base.Object;

namespace Test.Equip.Effect
{
    /// <summary>
    /// 装备效果接口
    /// </summary>
    public interface IEquipEffect
    {
        bool IsVaild { get; set; }
        
        void Apply(IEntityObject target);
        
        void Remove(IEntityObject target);
    }
}
