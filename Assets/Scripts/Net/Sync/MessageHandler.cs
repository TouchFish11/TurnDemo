using Core.DI;

namespace Net.Sync
{
    /// <summary>
    /// 消息处理器基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MessageHandler<T> : IMessageHandler where T : Message, new()
    {
        [Inject] protected INetGameProxy _netGameProxy;
        
        public abstract T Message { get; protected set; }
    
        public void HandleMessage(Message message)
        {
            Message = message as T;
            OnHandleMessage();
        }

        /// <summary>
        /// 处理具体消息逻辑
        /// </summary>
        protected abstract void OnHandleMessage();
    }
}
