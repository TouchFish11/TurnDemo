using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.AssetBundles.Update.Enum;
using Core.AssetBundles.Update.State;
using Core.Log;
using Core.Quit;
using Core.Service;
using Core.Singleton;
using Core.Utility;

namespace Core.AssetBundles.Update
{
    /// <summary>
    /// AssetBundle更新管理器
    /// 职责：负责AssetBundle全流程更新，包含远程列表下载、本地列表对比、资源下载、完整性校验等环节
    /// 设计模式：单例模式 + 状态模式（不同更新阶段封装为独立状态类）
    /// </summary>
    public class AssetBundleUpdater : SingletonBase<AssetBundleUpdater>, IAssetBundleUpdater
    {
        // 更新上下文（存储更新过程中的所有状态数据、下载任务、路径信息等）
        private ABUpdateContext _updateContext;
        // 更新状态字典（映射更新阶段与对应的状态处理类，实现状态模式的核心容器）
        private readonly Dictionary<EUpdatePhase, IUpdateState> _updateStateDic = new Dictionary<EUpdatePhase, IUpdateState>();
        // 当前更新状态（指向当前正在执行的更新阶段状态类）
        private IUpdateState _currentUpdateState;

        /// <summary>
        /// 私有构造函数（单例模式：禁止外部实例化）
        /// </summary>
        private AssetBundleUpdater()
        {

        }

        /// <summary>
        /// 初始化更新管理器
        /// 执行时机：游戏启动时初始化核心模块后调用
        /// 核心逻辑：1.注册应用退出回调 2.初始化本地路径 3.构建更新状态映射 4.初始化上下文
        /// </summary>
        public void Init()
        {
            // 注册应用退出事件，确保退出时能取消未完成的下载任务
            ServiceLocator.Get<IQuitHandler>().OnAppQuit += OnApplicationQuit;

            // 重置上下文（若存在旧上下文，清空历史数据）
            _updateContext?.ResetData();
            // 初始化AssetBundle本地存储路径及默认缓存文件
            InitLocalPath();

            // 初始化更新上下文（存储更新过程的核心数据）
            _updateContext = new ABUpdateContext();
            
            // 注册所有更新阶段对应的状态处理类（状态模式：每个阶段封装为独立State类，解耦各阶段逻辑）
            _updateStateDic.Add(EUpdatePhase.DownLoadRemoteListFile, new DownloadListFileState(this)); // 下载远程资源列表
            _updateStateDic.Add(EUpdatePhase.GetLocalCompareFile, new GetLocalListFileState(this));     // 获取本地资源列表（用于对比）
            _updateStateDic.Add(EUpdatePhase.CompareContrast, new CompareContrastState(this));           // 对比本地/远程列表，确定需要更新的资源
            _updateStateDic.Add(EUpdatePhase.DownLoadAssets, new DownLoadAssetState(this));             // 下载需要更新的AssetBundle资源
            _updateStateDic.Add(EUpdatePhase.CheckAssetsIntegrity, new CheckAssetIntegrityState(this)); // 校验下载完成的资源完整性（如MD5校验）
            _updateStateDic.Add(EUpdatePhase.Finished, new FinishState(this));                           // 更新完成状态
            _updateStateDic.Add(EUpdatePhase.NullState, new NullState(this));                             // 空状态（无操作，用于异常/终止场景）
        }

        /// <summary>
        /// 执行AssetBundle更新检查及下载流程
        /// 核心逻辑：按状态顺序执行各更新阶段，直到完成/暂停/异常
        /// </summary>
        /// <returns>更新流程是否执行成功（true=成功，false=异常终止）</returns>
        public async Task<bool> CheckUpdate()
        {
            // 初始切换到「下载远程列表」状态（更新流程的第一个阶段）
            ChangeState(EUpdatePhase.DownLoadRemoteListFile);

            // 循环执行当前状态的逻辑，直到满足退出条件：
            // 1. 暂停下载（外部主动暂停） 2. 当前状态为空（异常/终止） 3. 状态执行失败
            while (!_updateContext.IsPauseDownload && _currentUpdateState != null && _currentUpdateState != _updateStateDic[EUpdatePhase.NullState])
            {
                // 执行当前状态的核心逻辑（如下载列表、对比资源等）
                // 若某个状态执行失败，直接返回更新失败
                if (!await _currentUpdateState.Execute())
                {
                    return false;
                }
            }

            // 所有状态执行完成/正常退出，返回更新成功
            return true;
        }

        /// <summary>
        /// 初始化AssetBundle本地加载/缓存路径
        /// 核心逻辑：确保缓存标记文件存在，用于标识本地缓存目录的有效性
        /// </summary>
        private static void InitLocalPath()
        {
            // 获取缓存默认文件的完整路径（PathUtility封装了跨平台路径逻辑）
            var cacheFilePath = PathUtility.GetAbLoadPath(FileUtility.CacheDefaultName);
            // 若缓存标记文件不存在，则创建空文件（用于后续校验本地缓存目录是否有效）
            if (!File.Exists(cacheFilePath))
            {
                File.Create(cacheFilePath).Close(); // 创建文件后立即关闭，避免文件占用
            }
        }

        /// <summary>
        /// 切换更新状态（状态模式核心方法）
        /// 职责：退出当前状态 → 切换到目标状态 → 进入目标状态
        /// </summary>
        /// <param name="updatePhase">目标更新阶段</param>
        public void ChangeState(EUpdatePhase updatePhase)
        {
            // 从状态字典中获取目标状态类
            if (_updateStateDic.TryGetValue(updatePhase, out IUpdateState state))
            {
                // 退出当前状态（执行状态退出逻辑，如清理临时数据、释放资源）
                _currentUpdateState?.Exit();
                // 切换为目标状态
                _currentUpdateState = state;
                // 进入目标状态（执行状态初始化逻辑，如初始化下载参数、重置进度）
                _currentUpdateState.Enter();
            }
            else
            {
                // 日志报错：目标状态未注册（开发阶段排查状态枚举与字典的一致性）
                LogManager.LogError($"目标更新状态未注册；状态枚举：{updatePhase}");
            }
        }

        /// <summary>
        /// 获取更新上下文（提供外部访问更新过程数据的入口）
        /// </summary>
        /// <returns>更新上下文实例（包含下载进度、资源列表、错误信息等）</returns>
        public ABUpdateContext GetContext()
        {
            return _updateContext;
        }

        /// <summary>
        /// 应用退出时的回调处理
        /// 核心逻辑：取消所有未完成的下载任务，避免资源泄漏/异常
        /// </summary>
        /// <returns>异步任务（等待下载任务取消完成）</returns>
        private async Task OnApplicationQuit()
        {
            await _updateContext.CancelDownload();
        }
    }
}