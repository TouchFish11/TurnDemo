using HotUpdate.Base.ECModule;

namespace HotUpdate.Game.Battle.Property.New.Test.Equip.Effect
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
