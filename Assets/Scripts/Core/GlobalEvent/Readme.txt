事件中心：

1.EventCenter——管理所有事件的分发、监听，实现IEventCenter接口；
// 存储事件类型与对应事件信息列表的映射表。Key：事件类型（TEvent），Value：该类型下所有订阅的事件信息
private readonly Dictionary<Type, List<IEventInfo>> _typeToEventInfoMap = new();
// 延迟触发的事件队列，用于异步/分帧处理事件
private readonly Queue<IEvent> _delayEventQueue = new();
private readonly int _eventTriggerMaxNumPerFrame;
// 当前帧已触发的延迟事件数量，用于控制单帧触发上限
private byte _currentTriggeredEventCount;

/// <summary>
/// 私有构造函数（单例模式）
/// 初始化：注册Update监听，用于每帧处理延迟事件队列
/// </summary>
private EventCenter(IMonoAdapter monoAdapter)
{
    monoAdapter.AddUpdateListener(OnUpdate);
    _eventTriggerMaxNumPerFrame = GlobalSettings.Instance.eventTriggerMaxNumPerFrame;
}

2.IEventCenter——接口
/// <summary>
/// 立即触发指定类型的事件
/// </summary>
/// <typeparam name="TEvent">事件类型，必须实现IEvent接口</typeparam>
/// <param name="evt">待触发的事件实例</param>
void TriggerEvent<TEvent>(TEvent evt) where TEvent : IEvent;

/// <summary>
/// 延迟触发指定类型的事件
/// 可通过过滤器条件控制是否最终执行回调
/// </summary>
/// <typeparam name="TEvent">事件类型，必须实现IEvent接口</typeparam>
/// <param name="evt">待触发的事件实例</param>
void TriggerEventAsync<TEvent>(TEvent evt) where TEvent : IEvent;

/// <summary>
/// 订阅指定类型的事件
/// 当该类型事件触发时，符合过滤条件的回调会被执行
/// </summary>
/// <typeparam name="TEvent">事件类型，必须实现IEvent接口</typeparam>
/// <param name="callBack">事件触发时执行的回调方法</param>
/// <param name="filter">可选的事件过滤器，返回true时才执行回调，默认null（不过滤）</param>
void SubscribeEvent<TEvent>(Action<TEvent> callBack, Func<TEvent, bool> filter = null) where TEvent : IEvent;

/// <summary>
/// 取消订阅指定类型事件的指定回调方法
/// </summary>
/// <typeparam name="TEvent">事件类型，必须实现IEvent接口</typeparam>
/// <param name="callBack">需要取消订阅的回调方法</param>
/// <param name="filter"></param>
void UnsubscribeEvent<TEvent>(Action<TEvent> callBack, Func<TEvent, bool> filter = null) where TEvent : IEvent;

/// <summary>
/// 移除指定类型事件的所有订阅回调
/// 清空该事件类型下的所有监听逻辑
/// </summary>
/// <typeparam name="TEvent">事件类型，必须实现IEvent接口</typeparam>
void RemoveEventsFrom<TEvent>() where TEvent : IEvent;

3.IEventInfo——事件信息接口，仅占位，让泛型子类能统一存储
/// <summary>
/// 事件信息接口
/// </summary>
internal interface IEventInfo
{

}

4.EventInfo<TEvent> : IEventInfo, IPoolData where TEvent : IEvent——事件信息封装类，用于存储特定类型事件的回调方法和过滤条件；
/// <summary>
/// 事件触发时执行的回调方法
/// </summary>
public Action<TEvent> CallBack { get; set; }

/// <summary>
/// 事件执行回调前的过滤条件
/// 返回true则执行回调，返回false则跳过
/// </summary>
public Func<TEvent, bool> Filter { get; set; }

/// <summary>
/// 触发事件回调（执行前会先通过过滤条件校验）
/// </summary>
/// <param name="info">待处理的事件实例</param>
public void Invoke(TEvent info)
{
    // 先执行过滤条件，只有过滤通过才执行回调
    if (Filter == null || Filter.Invoke(info))
    {
        // 回调方法不为空时执行
        CallBack?.Invoke(info);
    }
}

public void ResetData()
{
    CallBack = null;
    Filter = null;
}

5.IEvent——事件接口
/// <summary>
/// 重置事件
/// </summary>
void ResetEvent();

6.abstract class Event : IEvent, IPoolData——事件中心模块的事件基类，所有自定义事件需继承此类；
实现接口；

7.EventFactory——事件工厂类，负责事件对象的创建与复用管理；
[Inject] private IPoolManager _poolManager; 

public TEvent GetEvent<TEvent>() where TEvent : Event
{
    return _poolManager.GetData<TEvent>();
}
