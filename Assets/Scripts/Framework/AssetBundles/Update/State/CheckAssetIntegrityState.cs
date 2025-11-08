using System.Collections;
using System.IO;

namespace Framework
{
    /// <summary>
    /// 检查资源完整性状态
    /// </summary>
    public class CheckAssetIntegrityState : UpdateState
    {
        public CheckAssetIntegrityState(AssetBundleUpdateManager manager) : base(manager)
        {

        }

        public override IEnumerator Execute()
        {
            //检查资源完整性
            yield return assetBundleUpdateManager.CheckAssetsIntegrity(isOver => IsSuceess = isOver,
            (cureent, total) =>
            {
                assetBundleUpdateManager.TransmitCheckProgress(cureent, total);
            });

            if (!IsSuceess)
            {
                LogMgr.LogError("资源不完整，请重新下载缺失资源");
                assetBundleUpdateManager.FinishUpdate(UpdatePhase);
                yield break;
            }

            //更新本地的AB包对比文件
            File.Copy(PathManager.GetAbLoadPath(FileUtility.TempCompareFileDefaultName), PathManager.GetAbLoadPath(FileUtility.CompareFileDefaultName), true);
            //删除临时记录文件
            //File.Delete(PathManager.GetAbLoadPath(FileUtility.RecordDefaultName));
            //删除临时对比文件
            File.Delete(PathManager.GetAbLoadPath(FileUtility.TempCompareFileDefaultName));
            //写入记录文件
            assetBundleUpdateManager.WriteAllRecordFile();
            //切换为完成状态
            assetBundleUpdateManager.ChangeState(E_UpdatePhase.Finished);
        }

        public override E_UpdatePhase UpdatePhase => E_UpdatePhase.CheckAssetsIntegrity;
    }
}
