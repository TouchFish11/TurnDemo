using System;
using System.Collections.Generic;
using Core.Log;
using Core.Singleton;

namespace Core.Dependence
{
    /// <summary>
    /// 依赖管理器
    /// TODO：未实现循环依赖、一对多的依赖，暂时只能一对一
    /// </summary>
    public class DependencyManager : SingletonBase<DependencyManager>, IDependencyManager
    {
        private readonly Dictionary<Type, List<IDependable>> _dependables = new();
        
        private DependencyManager()
        {
            
        }

        public async void Notice(Type notifyer)
        {
            if (_dependables.TryGetValue(notifyer, out var receivers))
            {
                foreach (var receiver in receivers)
                {
                    await receiver.OnDependcyInited();
                    LogManager.Log($"{nameof(DependencyManager)}.{nameof(Notice)}：依赖初始化完成。{receiver.GetType().Name}依赖于{notifyer.Name}");
                }
            }
        }

        public void RegisterDependable(Type dependable, IDependable receiver)
        {
            if (!_dependables.ContainsKey(dependable))
            {
                _dependables.Add(dependable, new List<IDependable> { receiver });
            }
            else
            {
                _dependables[dependable].Add(receiver);
            }
        }
    }
}
