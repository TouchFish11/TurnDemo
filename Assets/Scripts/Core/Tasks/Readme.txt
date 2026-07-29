自定义Task：

1.FTask——无返回值自定义任务基类，围绕 Unity 的 AsyncOperation 系列异步操作，实现了 类 Task 风格的异步等待和取消，实现对象池数据接口，可复用；

2.FTask<TResult> : FTask——有返回值自定义泛型任务类；
继承链：
AssetBundleRequestTask<TResult> : FTask<TResult>——AssetBundle单个资源请求任务类；
AssetBundleCreateRequestTask : FTask<AssetBundle>——AssetBundle创建请求的异步任务封装类；
AssetBundleRequestsTask<T> : FTask<IReadOnlyList<T>>——AB包批量请求资源任务；
UnityWebRequestAsyncOperationTask : FTask——UnityWebRequest异步操作任务；

3.struct FTaskAwaiter : ICriticalNotifyCompletion——自定义Task无返回值等待器；

4.struct FTaskAwaiter<TResult> : ICriticalNotifyCompletion——自定义Task有返回值等待器；

5.AssetBundleUnloadOperationTask : FTask——AssetBundle卸载操作的任务封装类；

6.TaskFactory——任务工厂：用于创建/复用各类AssetBundle相关任务实例；
/// <summary>
/// 创建AssetBundle创建请求任务
/// </summary>
/// <param name="req">AB创建请求</param>
/// <param name="token">取消令牌</param>
/// <returns>AB创建请求任务实例</returns>
public AssetBundleCreateRequestTask Create(AssetBundleCreateRequest req, CancellationToken token = default)
{
    var assetBundleCreateRequestTask = _poolManager.GetData<AssetBundleCreateRequestTask>();
    assetBundleCreateRequestTask.Init(req, token);
    return assetBundleCreateRequestTask;
}

/// <summary>
/// 创建泛型AssetBundle资源请求任务
/// </summary>
/// <typeparam name="T">资源类型</typeparam>
/// <param name="req">AB资源请求</param>
/// <param name="token">取消令牌</param>
/// <returns>泛型AB资源请求任务实例</returns>
public AssetBundleRequestTask<T> Create<T>(AssetBundleRequest req, CancellationToken token = default) where T : class
{
    var assetBundleRequestTask = _poolManager.GetData<AssetBundleRequestTask<T>>();
    assetBundleRequestTask.Init(req, token);
    return assetBundleRequestTask;
}

// 等等...

7.TaskAwaiterExtensions——任务等待器拓展类，为Unity的AssetBundle相关异步操作提供Task封装拓展方法，方便异步等待和取消；
private static TaskFactory s_taskFactory;
internal static void Configure(TaskFactory effectFactory)
{
    s_taskFactory = effectFactory;
}

/// <summary>
/// 将AssetBundleCreateRequest异步请求封装为可等待的Task
/// </summary>
/// <param name="req">AssetBundle创建请求实例</param>
/// <param name="token">取消令牌，可选参数，用于取消异步操作</param>
/// <returns>封装后的AssetBundleCreateRequestTask任务实例</returns>
public static TaskHandle<AssetBundle> ToTask(this AssetBundleCreateRequest req, CancellationToken token = default)
{
    var task = s_taskFactory.Create(req, token);
    return new TaskHandle<AssetBundle>(task);
}

/// <summary>
/// 将泛型AssetBundleRequest异步请求封装为可等待的泛型Task
/// </summary>
/// <typeparam name="T">加载的资源类型，继承自UnityEngine.Object</typeparam>
/// <param name="req">AssetBundle资源请求实例</param>
/// <param name="token">取消令牌，可选参数，用于取消异步操作</param>
/// <returns>封装后的泛型AssetBundleRequestTask任务实例</returns>
public static TaskHandle<T> ToTask<T>(this AssetBundleRequest req, CancellationToken token = default) where  T : class
{
    var task = s_taskFactory.Create<T>(req, token);
    return new TaskHandle<T>(task);
}

/// <summary>
/// 将泛型AssetBundleRequest异步请求封装为可等待的泛型Task
/// </summary>
/// <typeparam name="T">加载的资源类型，继承自UnityEngine.Object</typeparam>
/// <param name="req">AssetBundle资源请求实例</param>
/// <param name="token">取消令牌，可选参数，用于取消异步操作</param>
/// <returns>封装后的泛型AssetBundleRequestTask任务实例</returns>
public static TaskHandle<IReadOnlyList<T>> ToTasks<T>(this AssetBundleRequest req, CancellationToken token = default) where  T : class
{
    var task = s_taskFactory.Creates<T>(req, token);
    return new TaskHandle<IReadOnlyList<T>>(task);
}

/// <summary>
/// 将AssetBundleUnloadOperation卸载操作封装为可等待的Task
/// </summary>
/// <param name="req">AssetBundle卸载操作实例</param>
/// <returns>封装后的AssetBundleUnloadOperationTask任务实例</returns>
public static TaskHandle ToTask(this AssetBundleUnloadOperation req)
{
    var task = s_taskFactory.Create(req);
    return new TaskHandle(task);
}

/// <summary>
/// 将UnityWebRequestAsyncOperation操作封装为可等待的Task
/// </summary>
/// <param name="req"></param>
/// <param name="token"></param>
/// <returns></returns>
public static TaskHandle ToTask(this UnityWebRequestAsyncOperation req, CancellationToken token = default)
{
    var task = s_taskFactory.Create(req, token);
    return new TaskHandle(task);
}

8.struct TaskHandle : IDisposable——任务句柄；
// 句柄ID，调试用
private readonly int _id;
// 任务的引用计数
private uint _refCount; 
// 任务对象
private FTask _task;

/// <summary>
/// 获取内部的任务对象，每次访问该属性会是的引用计数增加
/// </summary>
public FTask Task
{
    get
    {
        ++_refCount;
        return _task;
    }
}

/// <summary>
/// 内部的任务对象是否有效
/// </summary>
public bool IsValid => _task != null;

public TaskHandle(FTask fTask)
{
    _id = TaskHandleHelper.GetGlobalId();
    _task = fTask;
    _refCount = 0;
}

/// <summary>
/// 减少引用计数，销毁句柄，销毁要和访问次数配对
/// </summary>
public void Dispose()
{
    if (_refCount > 0)
    {
        --_refCount;
    }
    
    if (_refCount == 0)
    {
        _task?.Release();
        _task = null;
    }
}

9.struct TaskHandle<T> : IDisposable——泛型任务句柄；
类似TaskHandle；


10.TaskHandleHelper——任务句柄辅助器；
// 任务句柄全局ID，调试使用
private static int _taskHandleGlobalId;

/// <summary>
/// 获取任务句柄全局ID，不复用
/// </summary>
/// <returns></returns>
public static int GetGlobalId()
{
    return ++_taskHandleGlobalId;
}

11.TaskUtility——任务工具类；
包含：
Task WaitUntil(Func<bool> condition);
IEnumerator WaitForTask(Task task);
IEnumerator WaitForTask<T>(Task<T> task, Action<T> callback);
Task WaitForCoroutine(IEnumerator coroutine, IMonoAdapter monoAdapter);
Task WaitForCoroutine(Coroutine coroutine, IMonoAdapter monoAdapter);