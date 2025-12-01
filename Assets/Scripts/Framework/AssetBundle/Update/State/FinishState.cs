using System.Threading.Tasks;

namespace Framework
{
    /// <summary>
    /// 完成状态
    /// </summary>
    public class FinishState : UpdateState
    {
        public FinishState(AssetBundleUpdater updater) : base(updater)
        {

        }

        public override Task<bool> Execute()
        {
            IsSuceess = true;
            // 删除缓存文件
            //if (File.Exists(PathManager.GetAbLoadPath(FileUtility.CacheDefaultName)))
            //{
            //    File.Delete(PathManager.GetAbLoadPath(FileUtility.CacheDefaultName));
            //}
            // 执行更新完成事件
            assetBundleUpdater.GetContext().UpdateFinish();

            // 切换到空状态
            assetBundleUpdater.ChangeState(E_UpdatePhase.NullState);
            return Task.FromResult(IsSuceess);
        }

        public override E_UpdatePhase UpdatePhase => E_UpdatePhase.Finished;
    }
}
