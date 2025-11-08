using Framework;
using System.Collections;

namespace Framework
{
    /// <summary>
    /// 对比差异状态
    /// </summary>
    public class CompareContrastState : UpdateState
    {
        public CompareContrastState(AssetBundleUpdateManager manager) : base(manager)
        {

        }

        public override IEnumerator Execute()
        {
            yield return assetBundleUpdateManager.CompareContrastFileInfo(isOver => IsSuceess = isOver);

            if (!IsSuceess)
            {
                LogMgr.LogError("差异对比失败");
                assetBundleUpdateManager.FinishUpdate(UpdatePhase);
                yield break;
            }

            assetBundleUpdateManager.ChangeState(E_UpdatePhase.DownLoadAssets);
        }

        public override E_UpdatePhase UpdatePhase => E_UpdatePhase.CompareContrast;
    }
}
