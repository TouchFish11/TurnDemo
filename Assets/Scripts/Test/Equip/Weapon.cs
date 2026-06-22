using HotUpdate.Base.Object;
using Test.Config;
using Test.Equip.Effect;

namespace Test.Equip
{
    /// <summary>
    /// 武器
    /// </summary>
    public class Weapon : Equipment
    {
        public Weapon(EquipmentConfig config, ITriggerCondition condition, IEquipEffect equipEffect) : base(config, condition, equipEffect)
        {
            
        }

        protected override void OnEquip(IEntityObject entityObject)
        {
            
        }

        protected override void OnUnEquip(IEntityObject entityObject)
        {
            
        }
    }
}
