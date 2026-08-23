namespace Core.GlobalEvent.Events.Net
{
    /// <summary>
    /// 玩家断开连接事件
    /// </summary>
    public class PlayerDisconnectedEvent : Event
    {
        /// <summary>
        /// 断开连接的客户端ID
        /// </summary>
        public int DisconnectionId { get; set; }
    }
}
