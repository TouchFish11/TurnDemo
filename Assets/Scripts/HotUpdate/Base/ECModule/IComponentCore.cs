using System;

namespace HotUpdate.Base.ECModule
{
    public interface IComponentCore : IDisposable
    {
        public void Init(IComponent component);
    }
}
