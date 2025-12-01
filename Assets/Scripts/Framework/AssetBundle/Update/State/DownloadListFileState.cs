using System.IO;
using System.Threading.Tasks;

namespace Framework
{
    /// <summary>
    /// 下载清单文件状态
    /// </summary>
    public class DownloadListFileState : UpdateState
    {
        public DownloadListFileState(AssetBundleUpdater updater) : base(updater)
        {

        }

        public override async Task<bool> Execute()
        {
            // 下载远端对比文件
            IsSuceess = await DownloadCompareFile();

            // 当前阶段未成功执行
            if (!IsSuceess)
            {
                LogMgr.LogError("远端对比文件下载失败");
                FinishUpdate();
                return IsSuceess;
            }

            // 获取远端AB包对比文件信息
            IsSuceess = await AnalyzeRemoteCompareFileInfo();
            if (!IsSuceess)
            {
                //记录日志
                LogMgr.LogError("AB包临时对比文件获取失败");
                FinishUpdate();
                return IsSuceess;
            }

            // 切换至读取本地文件状态
            assetBundleUpdater.ChangeState(E_UpdatePhase.GetLocalCompareFile);
            return IsSuceess;
        }

        /// <summary>
        /// 下载远端AB包对比文件
        /// </summary>
        /// <param name="overCallBack">下载结束回调</param>
        public async Task<bool> DownloadCompareFile()
        {
            // 创建web请求对象
            ABWebRequester aBWebRequester = new ABWebRequester(GlobalSettings.Instance.ResServerIp, FileUtility.ListFileDefaultName, false, string.Empty, string.Empty);
            for (int i = 0; i < GlobalSettings.Instance.ReDownloadCompareFileMaxNum; i++)
            {
                TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();
                // 下载
                aBWebRequester.DownLoadAsync(PathManager.GetAbLoadPath(FileUtility.TempListFileDefaultName), (isOver) =>
                {
                    IsSuceess = isOver;
                    source.SetResult(IsSuceess);
                });
                await source.Task;

                // 成功
                if (IsSuceess)
                {
                    return true;
                }
            }

            // 循环结束都没有成功，传递false
            if (!IsSuceess)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 解析远端AB包对比文件信息
        /// </summary>
        /// <returns>是否获取成功</returns>
        public async Task<bool> AnalyzeRemoteCompareFileInfo()
        {
            // 本地有该文件才去读取
            if (File.Exists(PathManager.GetAbLoadPath(FileUtility.TempListFileDefaultName)))
            {
                // 读取已经下载的AB包临时清单文件
                string listInfo = await File.ReadAllTextAsync(PathManager.GetAbLoadPath(FileUtility.TempListFileDefaultName));
                // 解析文件信息到远端下载的AB包信息集合
                AnalyzeCompareFileInfo(listInfo, E_FileAnalyzeType.Remote);
                return true;
            }
            return false;
        }

        public override E_UpdatePhase UpdatePhase => E_UpdatePhase.DownLoadRemoteListFile;
    }
}
