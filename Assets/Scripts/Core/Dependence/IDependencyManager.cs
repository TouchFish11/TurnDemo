using System;

namespace Core.Dependence
{
    public interface IDependencyManager
    {
        void Notice(Type notifyer);
        
        void RegisterDependable(Type dependable, IDependable receiver);
    }
}
