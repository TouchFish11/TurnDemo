using HotUpdate.Base.ECModule;

namespace HotUpdate.Game.Battle.Property.New.Test.Equip
{
    public interface ITriggerCondition
    {
        bool CanSatisfy(IEntityObject entityObject);
    }
}
