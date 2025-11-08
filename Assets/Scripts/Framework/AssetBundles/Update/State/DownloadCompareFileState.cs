using System.Collections;

namespace Framework
{
    /// <summary>
    /// 下载远端对比文件状态
    /// </summary>
    public class DownloadCompareFileState : UpdateState
    {
        public DownloadCompareFileState(AssetBundleUpdateManager manager) : base(manager)
        {

        }

        public override IEnumerator Execute()
        {
            //下载远端对比文件
            yield return assetBundleUpdateManager.DownloadCompareFile(isOver => IsSuceess = isOver);

            //当前阶段未成功执行
            if (!IsSuceess)
            {
                LogMgr.LogError("远端对比文件下载失败");
                assetBundleUpdateManager.FinishUpdate(UpdatePhase);
                yield break;
            }

            //获取远端AB包对比文件信息
            IsSuceess = assetBundleUpdateManager.AnalyzeRemoteCompareFileInfo();
            if (!IsSuceess)
            {
                //记录日志
                LogMgr.LogError("AB包临时对比文件获取失败");
                assetBundleUpdateManager.FinishUpdate(UpdatePhase);
                yield break;
            }

            assetBundleUpdateManager.ChangeState(E_UpdatePhase.GetLocalCompareFile);
        }

        public override E_UpdatePhase UpdatePhase => E_UpdatePhase.DownLoadRemoteCompareFile;
    }
}
