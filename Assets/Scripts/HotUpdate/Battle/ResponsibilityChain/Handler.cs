namespace HotUpdate.Battle.ResponsibilityChain
{
    /// <summary>
    /// 处理器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Handler<T>
    {
        // 持有下一个处理者的引用
        protected Handler<T> successor;

        // 设置下一个处理者
        public void SetSuccessor(Handler<T> successor)
        {
            this.successor = successor;
        }

        // 抽象处理请求方法
        public abstract void HandleRequest(T request);
    }
}
