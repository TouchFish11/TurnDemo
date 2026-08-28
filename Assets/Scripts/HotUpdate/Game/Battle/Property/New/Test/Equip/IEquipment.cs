using HotUpdate.Base.ECModule;
using Test.Config;

namespace HotUpdate.Game.Battle.Property.New.Test.Equip
{
    /// <summary>
    /// 装备接口
    /// </summary>
    public interface IEquipment
    {
        /// <summary>
        /// 装备配置
        /// </summary>
        EquipmentConfig Config { get; }

        /// <summary>
        /// 装备
        /// </summary>
        /// <param name="entityObject"></param>
        void Equip(IEntityObject entityObject);
        
        /// <summary>
        /// 卸下
        /// </summary>
        /// <param name="entityObject"></param>
        void UnEquip(IEntityObject entityObject);
    }
}
