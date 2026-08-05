using System;
using System.Collections.Generic;

namespace Core.DI
{
    /// <summary>
    /// 绑定构建器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindingBuilder<T> where T : class
    {
        private readonly List<Type> _bindTypes = new() { typeof(T) };
        private readonly Type _implementationType = typeof(T);

        /// <summary>
        /// 添加一个可解析的服务类型（接口或基类）
        /// </summary>
        public BindingBuilder<T> As<TService>() where TService : class
        {
            if (!typeof(TService).IsAssignableFrom(_implementationType))
                throw new Exception($"{_implementationType.Name} does not implement {typeof(TService).Name}");
            
            if (!_bindTypes.Contains(typeof(TService)))
                _bindTypes.Add(typeof(TService));
            return this;
        }
        
        /// <summary>
        /// 注册为单例（自动合并重复实现）
        /// </summary>
        public void AsSingleton()
        {
            var info = new BindingInfo { ImplementationType = _implementationType };
            // 注册时使用临时列表 _bindTypes，但 info 本身不存储它
            DIContainer.RegisterSingleton(info, _bindTypes);
        }

        /// <summary>
        /// 注册为瞬态
        /// </summary>
        public void AsTransient()
        {
            foreach (var type in _bindTypes)
            {
                DIContainer.RegisterTransient(type, _implementationType);
            }
        }
    }
}