using System.Collections.Generic;
using System.IO;
using Core.AssetBundles.Update.State;
using Core.DI;
using Core.Log;
using Core.Mono;
using Core.Pool;
using Core.Utility;

namespace Core.AssetBundles.Update.Core
{
    /// <summary>
    /// AssetBundle更新管理器
    /// </summary>
    public class AssetBundleUpdater : IAssetBundleUpdater, IApplicationExitNotify
    {
        public int QuitPriority => 0;
        
        [Inject] private UpdateResultFactory _updateResultFactory;
        // 对象池管理器接口
        private readonly IPoolManager _poolManager;
        // 更新上下文
        private ABUpdateContext _updateContext;
        // 更新状态映射
        private readonly Dictionary<EUpdatePhase, IUpdateState> _updateStateMap = new();
        // 当前更新状态
        private IUpdateState _currentUpdateState;
        
        /// <summary>
        /// 更新服务
        /// </summary>
        public UpdateService UpdateService { get; }

        /// <summary>
        /// 更新阶段
        /// </summary>
        public EUpdatePhase UpdatePhase => _currentUpdateState?.UpdatePhase ?? EUpdatePhase.None;

        private AssetBundleUpdater(IMonoAdapter monoAdapter, IPoolManager poolManager, UpdateService updateService)
        {
            monoAdapter.AddApplicationExitNotify(this);
            UpdateService = updateService;
            _poolManager = poolManager;
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
            foreach (var updateState in UpdateStateFactory.GetStates())
            {
                _updateStateMap.Add(updateState.UpdatePhase, updateState);
            }
        }

        /// <summary>
        /// 执行AssetBundle更新检查及下载流程
        /// </summary>
        public void CheckUpdate()
        {
            ChangePhase(EUpdatePhase.DownLoadRemoteCatalogFile);
        }
        
        public void ChangePhase(EUpdatePhase updatePhase)
        {
            if (_updateContext.IsPauseDownload)
                return;
                
            _currentUpdateState?.Exit();
            if (_updateStateMap.TryGetValue(updatePhase, out var updateState))
            {
                _currentUpdateState = updateState;
                _currentUpdateState.Enter();
            }
            else
            {
                // 该状态没有启用
                _currentUpdateState = _updateStateMap[++updatePhase];
                _currentUpdateState.Enter();
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
            _updateStateMap.Clear();
        }
        
        public void OnAppQuit()
        {
            try
            {
                if (_currentUpdateState == null || UpdatePhase == EUpdatePhase.Finished) 
                    return;
                UpdateService.CancelDownload(_updateContext);
                Logger.LogDebug(ELogTags.HotUpdate, $"已取消下载");
            }
            catch (System.Exception e)
            {
                Logger.LogError(ELogTags.HotUpdate, $"取消下载错误,{e.Message})");
            }
        }
    }
}