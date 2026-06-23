namespace HotUpdate.Base.Component
{
    public class ComponentCore<T> : IComponentCore<T> where T : IComponent
    {
        public T Component { get; private set; }
        
        public void Init(IComponent component)
        {
            Component = (T)component;
            OnInit();
        }

        protected virtual void OnInit()
        {
            
        }

        public virtual void Dispose()
        {
            
        }
    }
}
