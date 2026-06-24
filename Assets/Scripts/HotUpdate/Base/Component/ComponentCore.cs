namespace HotUpdate.Base.Component
{
    public class ComponentCore<T> : IComponentCore<T> where T : IComponent
    {
        public T Component { get; private set; }
        
        private bool _isDisposed = false;
        
        public void Init(IComponent component)
        {
            Component = (T)component;
            OnInit();
        }

        protected virtual void OnInit()
        {
            
        }

        public void Dispose()
        {
            if (_isDisposed) 
                return;
            
            OnDispose();
            _isDisposed = true;
        }

        protected virtual void OnDispose()
        {
            
        }
    }
}
