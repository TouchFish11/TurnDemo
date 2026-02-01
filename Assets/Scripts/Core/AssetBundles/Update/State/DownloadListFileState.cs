using System.IO;
using System.Threading.Tasks;
using Core.AssetBundles.Update.Enum;
using Core.Global;
using Core.Log;
using Core.Utility;

namespace Core.AssetBundles.Update.State
{
    /// <summary>
    /// 下载远程清单文件状态类
    /// 负责从服务器下载最新的AssetBundle清单文件（支持重试），并解析到远程包集合
    /// </summary>
    public class DownloadListFileState : UpdateState
    {
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
        public override async Task<bool> Execute()
        {
            // 下载远程对比文件（清单文件）
            IsSuceess = await DownloadCompareFile();

            // 下载失败，终止更新
            if (!IsSuceess)
            {
                LogManager.LogError("远程对比文件下载失败");
                FinishUpdate();
                return IsSuceess;
            }

            // 解析远程清单文件内容
            IsSuceess = await AnalyzeRemoteCompareFileInfo();
            if (!IsSuceess)
            {
                LogManager.LogError("AB远程对比文件解析失败");
                FinishUpdate();
                return IsSuceess;
            }

            // 切换状态到获取本地清单文件阶段
            assetBundleUpdater.ChangeState(EUpdatePhase.GetLocalCompareFile);
            return IsSuceess;
        }

        /// <summary>
        /// 下载远程AssetBundle对比文件（清单文件）
        /// 支持配置重试次数，失败后重试
        /// </summary>
        /// <returns>是否下载成功</returns>
        public async Task<bool> DownloadCompareFile()
        {
            // 创建清单文件下载请求器（无需MD5校验，清单文件本身由服务器保证正确性）
            var aBWebRequester = new ABWebRequester(GlobalSettings.Instance.resServerIp, FileUtility.ListFileDefaultName, false, string.Empty, string.Empty);
            
            // 按配置的最大重试次数执行下载
            var maxRetry = GlobalSettings.Instance.reDownloadCompareFileMaxNum;
            for (var i = 0; i < maxRetry; i++)
            {
                var source = new TaskCompletionSource<bool>();
                // 异步下载到临时清单文件路径
                aBWebRequester.DownLoadAsync(PathUtility.GetAbLoadPath(FileUtility.TempListFileDefaultName), (isOver) =>
                {
                    IsSuceess = isOver;
                    source.SetResult(IsSuceess);
                });
                await source.Task;

                // 下载成功，终止重试
                if (IsSuceess)
                {
                    return true;
                }
            }

            // 重试次数耗尽仍失败
            return false;
        }

        /// <summary>
        /// 解析远程下载的清单文件
        /// 将清单内容反序列化为远程包集合，供后续对比校验使用
        /// </summary>
        /// <returns>是否解析成功</returns>
        public async Task<bool> AnalyzeRemoteCompareFileInfo()
        {
            var tempListPath = PathUtility.GetAbLoadPath(FileUtility.TempListFileDefaultName);
            // 检查临时清单文件是否存在
            if (!File.Exists(tempListPath))
            {
                return false;
            }
            // 异步读取文件内容
            var listInfo = await File.ReadAllTextAsync(tempListPath);
            // 解析内容到远程包集合
            AnalyzeCompareFileInfo(listInfo, EFileAnalyzeType.Remote);
            return true;
        }

        /// <summary>
        /// 当前更新阶段标识
        /// </summary>
        public override EUpdatePhase UpdatePhase => EUpdatePhase.DownLoadRemoteListFile;
    }
}