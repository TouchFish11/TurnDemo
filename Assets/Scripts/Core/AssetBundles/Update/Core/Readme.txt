AB包更新模块：

下载流程：使用状态机模式将下载分为不同阶段，包含顺序：
1.DownloadCatalogState——下载远程目录文件状态类，负责从服务器下载最新的资源目录文件（支持重试），并解析到远程包集合。
2.LoadLocalCatalogFileState——获取本地目录文件状态类，负责加载本地资源目录文件（优先读取持久化路径，其次读取StreamingAssets），为后续对比校验做准备。
3.CompareContrastState——对比差异状态类，负责对比本地与远程AssetBundle包信息，确定需要下载/删除的资源，同时处理缓存文件校验。
4.CheckDeviceStorageState——检查设备存储状态，根据存储辅助器的方法（内部有平台判断，目前仅Window平台），使用外部函数来获取玩家存储数据，例如：
[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
private static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);
判断能否下载。
5.DownLoadAssetState——资源下载状态类，负责批量下载待更新的AssetBundle资源，支持并发下载、进度回调、下载速度更新。
6.CheckAssetIntegrityState——资源完整性校验状态类，校验已下载的AssetBundle资源的大小和Hash是否与远程一致（重新计算hash），处理冗余文件，完成后持久化缓存信息。
7.FinishState——更新完成状态类，处理更新完成后的收尾逻辑，标记更新结束。删除缓存文件，触发完成回调。

IUpdateState——资源包更新状态接口，定义了资源包更新流程中每个状态节点需要实现的核心行为。Enter、Execute（返回值是Task<UpdateResult>）、Exit

UpdateResult——更新结果，用于每个阶段的Execute方法的返回值，下载正常或异常被捕获后都返回这个UpdateResult。包含EUpdateError、异常封装、是否成功。

UpdateResultFactory——更新结果工厂，封装成功/失败的结果获取。

UpdateStateFactory——更新状态工厂，封装阶段状态类的实例创建，提供唯一入口获取所有“启用”的阶段——通过获取阶段枚举上的特性来判断是否创建该阶段实例并排序。

EUpdatePhase——下载更新阶段枚举，定义下载的所有阶段类型，枚举项被特性标记UpdateStateConfigAttribute标记。

UpdateStateConfigAttribute——更新状态配置特性，包含执行顺序（固定阶段的执行顺序）和是否启用（是否执行该阶段）。

AssetBundleUpdater——AssetBundle更新管理器，即状态机，管理、驱动状态的执行，对外提供唯一的检查更新方法，监听应用程序退出事件（应对中途退出）。
包含：
[Inject] private UpdateResultFactory _updateResultFactory;
// 对象池管理器接口
private readonly IPoolManager _poolManager;
// 更新上下文
private ABUpdateContext _updateContext;
// 更新状态列表
private readonly List<IUpdateState> _updateStates = new();
// 当前更新状态
private IUpdateState _currentUpdateState;
// 当前更新状态索引
private int _stateIndex;

/// <summary>
/// 更新服务
/// </summary>
public UpdateService UpdateService { get; }

/// <summary>
/// 更新阶段
/// </summary>
public EUpdatePhase UpdatePhase => _currentUpdateState?.UpdatePhase ?? EUpdatePhase.None;

ABUpdateContext——更新上下文，可通过对象池复用，统一管理下载时的所有相关状态，如解析的服务器目录集合、解析的本地目录集合、解析的缓存文件集合、存储等待下载的AB包信息集合、存储等待下载的网络请求链表、存储下载失败的网络请求链表、存储正在下载的网络请求链表、是否暂停下载、下载进度回调事件、资源检查进度回调事件、更新阶段变更回调事件、下载速度回调事件、更新结束回调事件、是否存在更新和一些便捷方法。

UpdateService——更新服务，提供下载使用的相关方法，如：持久化缓存信息，用于下次启动时断点续传、取消所有下载请求并保存缓存信息、更新AB包缓存文件信息、处理失败链表中的请求、终止所有正在下载的请求。

ABWebRequester——AssetBundle资源网络请求器，负责通过UnityWebRequest下载AB包资源，支持断点续传、下载超时检测、下载进度回调、重试次数管理等功能；实现IDisposable接口，用于释放UnityWebRequest相关资源，对UnityWebRequest的封装。

DownloadHandlerStream——流下载处理器，继承Unity DownloadHandlerScript自定义下载处理，使用FileStream创建、追加（用于断点续传）、关闭写入文件。

异常相关——定义了下载更新的所有相关异常。
包含：
UpdateException——更新异常基类，继承Exception。下面是子类。
AssetBunleBrokenException；
AssetBunleIncompleteException；
DownloadFailureException；
DriveShortageInsufficientException；
FileParsingException；
LocalListFileHandleException；


断点续传设计：通过三个文件实现，服务器目录、本地目录、缓存文件。
1：服务器和本地目录对比获取要更新的包；
2：要更新的包和缓存文件中比较，判断是否下载过了——
2.1下载完成但是没有进行校验那就不用下载了（这里有bug，若下载完成的包是旧的，由于不知道hash，新包会被跳过，需要在校验状态中处理，切回到对比差异状态）；
2.2下载未完成/没有缓存则继续下载/新下载；
2.3缓存中该包Hash与待下载包不一致，说明需要更新也要下载覆盖；



