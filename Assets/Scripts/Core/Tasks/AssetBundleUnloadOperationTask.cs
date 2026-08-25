namespace Core.Tasks
{
    /// <summary>
    /// AssetBundle卸载操作的任务封装类
    /// </summary>
    internal class AssetBundleUnloadOperationTask : AoTask
    {
        public override void Dispose()
        {
            poolManager.PushData(this);
        }
    }
}