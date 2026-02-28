using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Core.AssetBundles.Update.Enum;
using Core.AssetBundles.Update.Exception;
using Core.Global;
using Core.Mono;
using Core.Pool;
using Core.Service;
using Core.Utility;
using UnityEngine;

namespace Core.AssetBundles.Update.State
{
    /// <summary>
    /// 下载远程清单文件状态类
    /// 负责从服务器下载最新的AssetBundle清单文件（支持重试），并解析到远程包集合
    /// </summary>
    public class DownloadListFileState : UpdateState
    {
        private ABWebRequester _abWebRequester;
        private Coroutine _coroutine;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="updater">AssetBundle更新器实例</param>
        public DownloadListFileState(AssetBundleUpdater updater) : base(updater)
        {
        }

        /// <summary>
        /// 执行下载远程清单文件核心逻辑
        /// </summary>
        /// <returns>是否执行成功</returns>
        public override async Task<UpdateResult> Execute()
        {
            try
            {
                // 下载远程清单文件
                await DownloadCompareFile();
                // 解析远程清单文件内容
                await AnalyzeRemoteCompareFileInfo();
            }
            catch (DownloadFailureException downloadFailureException)
            {
                return UpdateResult.CreateFailure(UpdateResult.EUpdateError.DownloadFailure, downloadFailureException);
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                return UpdateResult.CreateFailure(UpdateResult.EUpdateError.LocalListFile, fileNotFoundException);
            }
            catch (System.Exception exception)
            {
                return UpdateResult.CreateFailure(UpdateResult.EUpdateError.Unknown, exception);
            }
            
            return UpdateResult.CreateSuccess();
        }

        /// <summary>
        /// 下载远程AssetBundle对比文件
        /// 支持配置重试次数，失败后重试
        /// </summary>
        /// <returns>是否下载成功</returns>
        public async Task DownloadCompareFile()
        {
            // 创建清单文件下载请求器（无需Hash校验，清单文件本身由服务器保证正确性）
            _abWebRequester = poolManager.GetData<ABWebRequester>().Init(GlobalSettings.Instance.resServerIp, FileUtility.ListFileDefaultName, false, string.Empty, string.Empty, 0);

            _coroutine = ServiceLocator.Get<IMonoAdapter>().StartCoroutine(CheckCancel());
            
            // 按配置的最大重试次数执行下载
            var maxRetry = GlobalSettings.Instance.reDownloadCompareFileMaxNum;
            for (var i = 0; i < maxRetry; i++)
            {
                var source = new TaskCompletionSource<bool>();
                // 异步下载到临时清单文件路径
                _abWebRequester.DownLoadAsync(PathUtility.GetAbLoadPath(FileUtility.TempListFileDefaultName), isOver => source.SetResult(isOver));
                var isSuceess = await source.Task;

                // 下载成功，终止重试
                if (!isSuceess)
                {
                    continue;
                }
                
                ServiceLocator.Get<IMonoAdapter>().StopCoroutine(_coroutine);
                _abWebRequester.Abort();
                ServiceLocator.Get<IPoolManager>().PushData(_abWebRequester);
                _abWebRequester = null;
                return;
            }

            // 重试次数耗尽仍失败
            throw new DownloadFailureException($"服务器清单文件下载失败，最大重试次数：{maxRetry}");
        }

        private IEnumerator CheckCancel()
        {
            while (!assetBundleUpdater.GetContext().IsPauseDownload)
            {
                yield return null;
            }
            _abWebRequester.Abort();
        }

        /// <summary>
        /// 解析远程下载的清单文件
        /// 将清单内容反序列化为远程包集合，供后续对比校验使用
        /// </summary>
        /// <returns>是否解析成功</returns>
        public async Task AnalyzeRemoteCompareFileInfo()
        {
            var tempListPath = PathUtility.GetAbLoadPath(FileUtility.TempListFileDefaultName);
            // 检查临时清单文件是否存在
            if (!File.Exists(tempListPath))
            {
                throw new FileNotFoundException($"未找到本地清单文件，路径：{tempListPath}");
            }
            // 异步读取文件内容
            var listInfo = await File.ReadAllTextAsync(tempListPath);
            // 解析内容到远程包集合
            AnalyzeCompareFileInfo(listInfo, EFileAnalyzeType.Remote);
        }

        /// <summary>
        /// 当前更新阶段标识
        /// </summary>
        public override EUpdatePhase UpdatePhase => EUpdatePhase.DownLoadRemoteListFile;
    }
}