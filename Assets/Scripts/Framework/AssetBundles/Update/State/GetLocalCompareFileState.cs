using System.Collections;

namespace Framework
{
    /// <summary>
    /// 获取本地对比文件状态
    /// </summary>
    public class GetLocalCompareFileState : UpdateState
    {
        public GetLocalCompareFileState(AssetBundleUpdateManager manager) : base(manager)
        {

        }

        public override IEnumerator Execute()
        {
            //获取本地对比文件
            yield return assetBundleUpdateManager.GetLocalCompareFileInfo(isOver => IsSuceess = isOver);
            if (!IsSuceess)
            {
                LogMgr.LogError("本地对比文件获取失败");
                assetBundleUpdateManager.FinishUpdate(UpdatePhase);
                yield break;
            }

            assetBundleUpdateManager.ChangeState(E_UpdatePhase.CompareContrast);
        }

        public override E_UpdatePhase UpdatePhase => E_UpdatePhase.GetLocalCompareFile;
    }
}
