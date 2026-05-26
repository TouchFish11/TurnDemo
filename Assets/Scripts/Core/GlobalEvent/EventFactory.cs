using Core.DI;
using Core.Pool;

namespace Core.GlobalEvent
{
    /// <summary>
    /// 事件工厂类，负责事件对象的创建与复用管理
    /// 限定泛型类型为 IEvent 接口实现类
    /// </summary>
    public class EventFactory : IEventFactory
    {
        [Inject] private IPoolManager _poolManager; 
        

        public TEvent GetEvent<TEvent>() where TEvent : Event
        {
            return _poolManager.GetData<TEvent>();;
        }
    }
}