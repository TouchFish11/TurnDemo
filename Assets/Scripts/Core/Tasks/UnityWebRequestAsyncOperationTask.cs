namespace Core.Tasks
{
    /// <summary>
    /// UnityWebRequest异步操作任务
    /// </summary>
    internal class UnityWebRequestAsyncOperationTask : AoTask
    {
        public override void Dispose()
        {
            poolManager.PushData(this);
        }
    }
}
