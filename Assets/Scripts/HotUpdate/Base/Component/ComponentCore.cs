using System;

namespace HotUpdate.Base.Component
{
    public class ComponentCore<T> : IComponentCore<T> where T : IComponent
    {
        public T Component { get; private set; }
        
        private bool _isDisposed;
        
        /// <summary>
        /// 初始化组件逻辑类，逻辑类初始化先于组件初始化
        /// </summary>
        /// <param name="component"></param>
        public void Init(IComponent component)
        {
            Component = (T)component;
            OnInit();
        }

        protected virtual void OnInit()
        {
            
        }

        void IDisposable.Dispose()
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
