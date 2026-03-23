using Core.Components;

namespace Test.Equip
{
    public interface ITriggerCondition
    {
        bool CanSatisfy(IEntityObject entityObject);
    }
}
