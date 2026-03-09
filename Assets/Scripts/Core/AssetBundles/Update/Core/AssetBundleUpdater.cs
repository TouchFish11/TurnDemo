using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.AssetBundles.Update.Enum;
using Core.Log;
using Core.Mono;
using Core.Pool;
using Core.Serialize.Json;
using Core.Service;
using Core.Singleton;
using Core.Utility;

namespace Core.AssetBundles.Update.Core
{
    /// <summary>
    /// AssetBundle更新管理器
    /// </summary>
    public class AssetBundleUpdater : SingletonBase<AssetBundleUpdater>, IAssetBundleUpdater
    {
        public override int Priority => 1;
        // 更新上下文
        private ABUpdateContext _updateContext;
        // 更新状态列表
        private readonly List<IUpdateState> _updateStates = new();
        // 当前更新状态
        private IUpdateState _currentUpdateState;
        // 当前更新状态索引
        private int _stateIndex;
        // 对象池管理器接口
        private IPoolManager _poolManager;

        /// <summary>
        /// 更新阶段
        /// </summary>
        public EUpdatePhase UpdatePhase => _currentUpdateState.UpdatePhase;
        
        private AssetBundleUpdater(){}
        
        public override Task InitAsync()
        {
            _poolManager = ServiceLocator.Get<IPoolManager>();
            // 注册应用退出事件，确保退出时能取消未完成的下载任务
            ServiceLocator.Get<IMonoAdapter>().OnAppQuit += OnApplicationQuit;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 初始化更新管理器
        /// </summary>
        public void Init()
        {
            ResetData();
            // 初始化AssetBundle本地存储路径及默认缓存文件
            InitLocalPath();
            // 初始化更新上下文
            _updateContext = _poolManager.GetData<ABUpdateContext>();

            var factory = new UpdateStateFactory(this, _poolManager, ServiceLocator.Get<IJsonManager>());
            foreach (var updateState in factory.GetStates())
            {
                _updateStates.Add(updateState);
            }
        }

        /// <summary>
        /// 执行AssetBundle更新检查及下载流程
        /// </summary>
        public async void CheckUpdate()
        {
            try
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

                    _updateContext.UpdateOver(result);
                    return;
                }
            }
            catch (System.Exception e)
            {
                LogManager.LogError($"{nameof(AssetBundleUpdater)}.{nameof(CheckUpdate)}：下载异常：{e.Message}");
                _updateContext.UpdateOver(UpdateResult.CreateFailure(UpdateResult.EUpdateError.Unknown, e));
            }
        }

        /// <summary>
        /// 初始化AssetBundle本地加载/缓存路径
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
        /// 获取更新上下文
        /// </summary>
        /// <returns>更新上下文实例（</returns>
        public ABUpdateContext GetContext()
        {
            return _updateContext;
        }

        private void ResetData()
        {
            if (_updateContext == null)
            {
                return;
            }
            
            _poolManager.PushData(_updateContext);
            _stateIndex = 0;
            _updateStates.Clear();
        }

        /// <summary>
        /// 应用退出时的回调处理
        /// </summary>
        private async Task OnApplicationQuit()
        {
            if (_currentUpdateState == null || UpdatePhase == EUpdatePhase.Finished)
            {
                return;
            }
            
            await _updateContext.CancelDownload();
        }
    }
}