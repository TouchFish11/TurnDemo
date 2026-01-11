using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// AssetBundle更新器
    /// </summary>
    public class AssetBundleUpdater : SingletonBase<AssetBundleUpdater>, IAssetBundleUpdater
    {
        // 更新上下文
        private ABUpdateContext _updateContext;
        // 更新状态字典
        private readonly Dictionary<E_UpdatePhase, IUpdateState> _updateStateDic = new Dictionary<E_UpdatePhase, IUpdateState>();
        // 当前更新状态
        private IUpdateState _currentUpdateState;

        private AssetBundleUpdater()
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            ServiceLocator.Get<IQuitHandler>().OnAppQuit += OnApplicationQuit;

            _updateContext?.ResetData();
            InitLocalPath();

            _updateContext = new ABUpdateContext();
            _updateStateDic.Add(E_UpdatePhase.DownLoadRemoteListFile, new DownloadListFileState(this));
            _updateStateDic.Add(E_UpdatePhase.GetLocalCompareFile, new GetLocalListFileState(this));
            _updateStateDic.Add(E_UpdatePhase.CompareContrast, new CompareContrastState(this));
            _updateStateDic.Add(E_UpdatePhase.DownLoadAssets, new DownLoadAssetState(this));
            _updateStateDic.Add(E_UpdatePhase.CheckAssetsIntegrity, new CheckAssetIntegrityState(this));
            _updateStateDic.Add(E_UpdatePhase.Finished, new FinishState(this));
            _updateStateDic.Add(E_UpdatePhase.NullState, new NullState(this));
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        public async Task<bool> CheckUpdate()
        {
            // 切换为第一个状态
            ChangeState(E_UpdatePhase.DownLoadRemoteListFile);

            // 没用暂停，当前状态不为空，当前状态不为完成，则继续更新
            while (!_updateContext.IsPauseDownload && _currentUpdateState != null && _currentUpdateState != _updateStateDic[E_UpdatePhase.NullState])
            {
                if (!await _currentUpdateState.Execute())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 初始化本地路径
        /// </summary>
        private void InitLocalPath()
        {
            // 没有缓存文件就创建缓存文件
            if (!File.Exists(PathUtility.GetAbLoadPath(FileUtility.CacheDefaultName)))
            {
                File.Create(PathUtility.GetAbLoadPath(FileUtility.CacheDefaultName)).Close();
            }
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="updatePhase"></param>
        public void ChangeState(E_UpdatePhase updatePhase)
        {
            if (_updateStateDic.TryGetValue(updatePhase, out IUpdateState state))
            {
                _currentUpdateState?.Exit();
                _currentUpdateState = state;
                _currentUpdateState.Enter();
            }
            else
            {
                // 目标状态未注册
                LogManager.LogError($"目标状态未注册；{updatePhase}");
            }
        }

        /// <summary>
        /// 获取上下文
        /// </summary>
        /// <returns></returns>
        public ABUpdateContext GetContext()
        {
            return _updateContext;
        }

        /// <summary>
        /// 在应用程序退出时
        /// </summary>
        /// <returns></returns>
        private async Task OnApplicationQuit()
        {
            await _updateContext.CancelDownload();
        }
    }
}
