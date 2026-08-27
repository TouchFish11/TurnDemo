using System.IO;
using System.Threading.Tasks;
using Core.AssetBundles.Update.Core;
using Core.AssetBundles.Update.Exception;
using Core.DI;
using Core.Global;
using Core.Log;
using Core.Mono;
using Core.Utility;
using Logger = Core.Log.Logger;

namespace Core.AssetBundles.Update.State
{
    /// <summary>
    /// 下载远程目录文件状态类
    /// 负责从服务器下载最新的资源目录文件（支持重试），并解析到远程包集合
    /// </summary>
    public class DownloadCatalogState : UpdateState
    {
        [Inject] private IMonoAdapter _monoAdapter;

        protected override async void OnEnter()
        {
            try
            {
                // 下载远程清单文件
                await DownloadCatalogFile();
                // 解析远程清单文件内容
                await AnalyzeRemoteCatalog();
                assetBundleUpdater.ChangePhase(EUpdatePhase.LoadLocalCatalogFile);
            }
            catch (DownloadFailureException downloadFailureException)
            {
                var result = updateResultFactory.CreateFailure(UpdateResult.EUpdateError.DownloadFailure, downloadFailureException);
                assetBundleUpdater.GetContext().UpdateOver(result);
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                var result =  updateResultFactory.CreateFailure(UpdateResult.EUpdateError.LocalListFile, fileNotFoundException);
                assetBundleUpdater.GetContext().UpdateOver(result);
            }
            catch (System.Exception exception)
            {
                var result =  updateResultFactory.CreateFailure(UpdateResult.EUpdateError.Unknown, exception);
                assetBundleUpdater.GetContext().UpdateOver(result);
            }
        }

        /// <summary>
        /// 下载远程资源目录文件
        /// 支持配置重试次数，失败后重试
        /// </summary>
        /// <returns>是否下载成功</returns>
        public async Task DownloadCatalogFile()
        {
            // 按配置的最大重试次数执行下载
            var maxRetry = GlobalSettings.Instance.updateModuleConfig.reDownloadCompareFileMaxNum;
            for (var i = 0; i < maxRetry; i++)
            {
                var isSuccess = await DownloadCatalogFileInternal();
                // 下载成功，终止重试
                if (isSuccess)
                {
                    return;
                }

                await Task.Yield();
            }
            
            // 重试次数耗尽仍失败
            throw new DownloadFailureException($"服务器清单文件下载失败，最大重试次数：{maxRetry}");
        }

        private Task<bool> DownloadCatalogFileInternal()
        {
            var source = new TaskCompletionSource<bool>();
            // 创建清单文件下载请求器（无需Hash校验，清单文件本身由服务器保证正确性）
            var abWebRequester = poolManager.GetData<ABWebRequester>().Init(GlobalSettings.Instance.updateModuleConfig.resServerIp, FileUtility.CatalogDefaultName, false, string.Empty, string.Empty, 0);
            // 异步下载到临时清单文件路径
            abWebRequester.DownLoadAsync(PathUtility.GetAbLoadPath(FileUtility.TempCatalogDefaultName), over =>
            {
                source.SetResult(over);
                poolManager.PushData(abWebRequester);
            }, GlobalSettings.Instance.updateModuleConfig.connectTimeout);
            return source.Task;
        }

        /// <summary>
        /// 解析远程下载的目录文件
        /// 将清单内容反序列化为远程包集合，供后续对比校验使用
        /// </summary>
        /// <returns>是否解析成功</returns>
        public async Task AnalyzeRemoteCatalog()
        {
            var tempCatalogPath = PathUtility.GetAbLoadPath(FileUtility.TempCatalogDefaultName);
            // 检查临时清单文件是否存在
            if (!File.Exists(tempCatalogPath))
            {
                throw new FileNotFoundException($"[{nameof(DownloadCatalogState)}]: Not found local directory file, path({tempCatalogPath})");
            }
            
            try
            {
                // 异步读取文件内容
                var catalogJson = await File.ReadAllTextAsync(tempCatalogPath);
                // 解析内容到远程包集合
                AnalyzeCatalog(catalogJson, EFileAnalyzeType.Remote);
            }
            catch (IOException ex)
            {
                Logger.LogError(ELogTags.HotUpdate, $"ReadAllTextAsync failed: {ex}");
                throw;
            }
        }

        protected override void OnExit()
        {

        }

        /// <summary>
        /// 当前更新阶段标识
        /// </summary>
        public override EUpdatePhase UpdatePhase => EUpdatePhase.DownLoadRemoteCatalogFile;
    }
}