using HotUpdate.Base.Object;

namespace Test.Equip
{
    public interface ITriggerCondition
    {
        bool CanSatisfy(IEntityObject entityObject);
    }
}
