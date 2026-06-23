using System;

namespace HotUpdate.Base.Component
{
    public interface IComponentCore<out T> : IDisposable where T : IComponent
    {
        T Component { get; }

        public void Init(IComponent component);
    }
}
