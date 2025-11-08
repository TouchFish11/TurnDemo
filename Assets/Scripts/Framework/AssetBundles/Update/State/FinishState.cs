using System.Collections;

namespace Framework
{
    /// <summary>
    /// Íê³É×´Ì¬
    /// </summary>
    public class FinishState : UpdateState
    {
        public FinishState(AssetBundleUpdateManager manager) : base(manager)
        {

        }

        public override IEnumerator Execute()
        {
            IsSuceess = true;
            yield return null;
            assetBundleUpdateManager.FinishUpdate(UpdatePhase);
        }

        public override E_UpdatePhase UpdatePhase => E_UpdatePhase.Finished;
    }
}
