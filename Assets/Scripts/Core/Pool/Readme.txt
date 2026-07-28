对象池：

1.IPool——定义mono/C#对象池的统一属性，方法；
/// <summary>
/// 池子ID——对象名称
/// </summary>
string PoolId { get; }

/// <summary>
/// 标记池是否惰性
/// </summary>
bool IsLazy { get; }

/// <summary>
/// 上次Get/Push的时间，越小则越早使用
/// </summary>
float LastUsedTime { get; }

/// <summary>
/// 使用对象数
/// </summary>
int ActiveCount { get; }

/// <summary>
/// 未使用的对象数
/// </summary>
int InactiveCount { get; }

/// <summary>
/// 清理池子所有缓存
/// </summary>
void ClearAll();

/// <summary>
/// 修剪池子——清理对象到最小容量
/// </summary>
void Trim();

2.IPool<T>——泛型接口，继承IPool，提供统一泛型方法；
/// <summary>
/// 获取
/// </summary>
/// <returns></returns>
T Get();

/// <summary>
/// 放入
/// </summary>
/// <param name="obj"></param>
void Push(T obj);

3.ObjectPool<T>——对象池，缓存继承Unity Object的对象，实现IPool<T>；
// 存储未使用对象的栈结构（栈结构适合后进先出的复用逻辑）
private readonly Stack<T> _unUsedMonos = new();
// 对象池的父物体（用于统一管理池内对象的层级）
private GameObject _parentObj;
// 是否开启布局管理
private readonly bool _isOpenLayout;
// 活跃时间阈值，大于等于该数值活跃，小于则惰性
private readonly float _activeTimeThreshold;
// 最小缓存数量
private readonly int _minSize;
// 最大缓存容量
private readonly int _maxSize;

4.DataPool<T>——C#对象池，缓存纯C#对象，实现IPool<T>；
// 存储未使用的数据对象队列
private readonly Queue<T> _unUsedDatas = new();
// 活跃时间阈值，大于等于该数值活跃，小于则惰性
private readonly float _activeTimeThreshold;
// 最小缓存数量
private readonly int _minSize;
// 最大缓存容量
private readonly int _maxSize;


5.PoolManager——对象池管理器，管理所有的池子对象，封装方法，实现IPoolManager、IMemoryListener接口；
// 对象名称到池子的缓存映射
private readonly Dictionary<string, IPool> _pools = new();
// 对象池的LRU链表
private readonly LinkedList<string> _lruPoolIds = new();
// 缓存池根对象
private GameObject _poolRootObj;
// 是否开启对象池布局
private readonly bool _isOpenLayout;
// 活跃时间阈值，大于该数值为惰性，小于为活跃
private readonly float activeTimeThreshold;
// 池子统一最小阈值
private readonly int poolMinSize;
// 池子统一最大阈值
private readonly int poolMaxSize;

// 构造获取全局设置，初始化
private PoolManager()
{
    _isOpenLayout = GlobalSettings.Instance.isOpenLayout;
    activeTimeThreshold = GlobalSettings.Instance.activeTimeThreshold;
    poolMinSize = GlobalSettings.Instance.poolMinSize;
    poolMaxSize = GlobalSettings.Instance.poolMaxSize;
}

包含：
私有方法InsertFirst——插入链表头，LRU；
下面是接口方法实现；

/// <summary>
/// 获取缓存的游戏对象，没有返回null
/// </summary>
/// <param name="key">资源Key</param>
/// <returns></returns>
T Get<T>(string key) where T : Object;

/// <summary>
/// 缓存游戏对象，超过最大容量则直接销毁
/// </summary>
/// <param name="obj">游戏对象</param>
void PushObj<T>(T obj) where T : Object;

/// <summary>
/// 缓存纯C#的对象，超过最大容量则不缓存
/// </summary>
/// <typeparam name="T">类名</typeparam>
/// <param name="data">数据对象</param>
void PushData<T>(T data) where T : class, IPoolData;

/// <summary>
/// 获取纯C#的对象，自动注入[Inject]依赖，复用对象不会触发构造函数，所以无法通过构造注入
/// </summary>
/// <typeparam name="T"></typeparam>
/// <returns></returns>
T GetData<T>() where T : class, IPoolData;

/// <summary>
/// 释放指定资源缓存，清空指定池子所有对象
/// </summary>
/// <param name="assetName">资源名称</param>
/// <returns>销毁的对象数量</returns>
int ReleaseCache(string assetName);

/// <summary>
/// 清空缓存池，清空指定所有池子对象
/// </summary>
void ClearAll();

/// <summary>
/// 获取指定资源缓存的数量
/// </summary>
/// <param name="assetName"></param>
/// <returns></returns>
int GetUnUsedCount(string assetName);

/// <summary>
/// 强制释放内存，可指定释放的选择策略
/// </summary>
/// <param name="disposalStrategy"></param>
/// <param name="executeCount">执行次数，即释放的池子数量</param>
void ReleaseCache(PoolManager.EReleaseStrategy disposalStrategy = PoolManager.EReleaseStrategy.Trim, ushort executeCount = 1);

6.EReleaseStrategy——释放策略；
/// <summary>
/// 裁剪所有池子到最小容量，调用所有池子的Trim方法。
/// </summary>
Trim,
/// <summary>
/// 根据executeCount决定释放池子数
/// </summary>
LRU,

7.IMemoryListener——内存监听器，对象池实现，能相应内存阈值事件回调，自动释放缓存。