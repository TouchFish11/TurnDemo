using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
    /// </summary>
    public class AssetBundleUpdater : SingletonBase<AssetBundleUpdater>, IAssetBundleUpdater
    {
        // 更新上下文
        private ABUpdateContext _updateContext;
        // 更新状态列表
        private readonly List<IUpdateState> _updateStates = new();
        // 当前更新状态
        private IUpdateState _currentUpdateState;
        //
        private int _stateIndex;

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
            
            // 注册所有更新阶段对应的状态处理类
            _updateStates.Add(new DownloadListFileState(this)); // 下载远程资源列表
            _updateStates.Add(new GetLocalListFileState(this)); // 获取本地资源列表（用于对比）
            _updateStates.Add(new CompareContrastState(this));  // 对比本地/远程列表，确定需要更新的资源
            _updateStates.Add(new DownLoadAssetState(this));    // 下载需要更新的AssetBundle资源
            _updateStates.Add(new CheckAssetIntegrityState(this));  // 校验下载完成的资源完整性（如Hash校验）
            _updateStates.Add(new FinishState(this));   // 更新完成状态

            _stateIndex = 0;
        }

        /// <summary>
        /// 执行AssetBundle更新检查及下载流程
        /// 核心逻辑：按状态顺序执行各更新阶段，直到完成/暂停/异常
        /// </summary>
        /// <returns>更新流程是否执行成功（true=成功，false=异常终止）</returns>
        public async Task CheckUpdate()
        {
            _currentUpdateState = _updateStates[_stateIndex];
            // 循环执行当前状态的逻辑，直到满足退出条件：
            // 暂停下载（外部主动暂停） 当前状态为空（异常/终止） 状态执行失败
            while (!_updateContext.IsPauseDownload && _currentUpdateState != null)
            {
                // 执行当前状态的核心逻辑
                _currentUpdateState.Enter();
                var result = await _currentUpdateState.Execute();
                if (result.Success)
                {
                    _currentUpdateState.Exit();

                    if (_stateIndex == _updateStates.Count - 1)
                    {
                        return;
                    }
                    _currentUpdateState = _updateStates[++_stateIndex];
                    continue;
                }
                
                _updateContext.UpdateFailed(result);
                return;
            }
        }

        /// <summary>
        /// 初始化AssetBundle本地加载/缓存路径
        /// 核心逻辑：确保缓存标记文件存在，用于标识本地缓存目录的有效性
        /// </summary>
        private static void InitLocalPath()
        {
            // 获取缓存默认文件的完整路径
            var cacheFilePath = PathUtility.GetAbLoadPath(FileUtility.CacheDefaultName);
            // 若缓存标记文件不存在，则创建空文件（用于后续校验本地缓存目录是否有效）
            if (!File.Exists(cacheFilePath))
            {
                File.Create(cacheFilePath).Close(); // 创建文件后立即关闭，避免文件占用
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