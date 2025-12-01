using System.Threading.Tasks;

namespace Framework
{
    /// <summary>
    /// 空状态（用于退出更新）
    /// </summary>
    public class NullState : UpdateState
    {
        public NullState(AssetBundleUpdater updater) : base(updater)
        {

        }

        public override Task<bool> Execute()
        {
            IsSuceess = true;
            return Task.FromResult(IsSuceess);
        }

        public override E_UpdatePhase UpdatePhase => E_UpdatePhase.NullState;
    }
}
