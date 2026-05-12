namespace Net.Sync
{
    /// <summary>
    /// 消息处理器
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="message"></param>
        void HandleMessage(Message message);
    }
}
