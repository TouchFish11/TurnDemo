using System.Collections;

namespace Framework
{
    /// <summary>
    /// 更新状态基类
    /// </summary>
    public abstract class UpdateState : IUpdateState
    {
        protected AssetBundleUpdateManager assetBundleUpdateManager;

        protected UpdateState(AssetBundleUpdateManager manager)
        {
            this.assetBundleUpdateManager = manager;
        }

        public virtual void Enter()
        {
            assetBundleUpdateManager.TransmitPhase(UpdatePhase);
        }

        public abstract IEnumerator Execute();

        public virtual void Exit()
        {

        }

        public abstract E_UpdatePhase UpdatePhase { get; }

        public bool IsSuceess { get; set; }
    }
}
