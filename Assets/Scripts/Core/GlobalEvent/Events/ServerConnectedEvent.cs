using Core.Net.Tcp.Message.S2C;

namespace Core.GlobalEvent.Events
{
    /// <summary>
    /// 连接服务器完成事件
    /// </summary>
    public class ServerConnectedEvent : Event
    {
        public S2C_ConnectMessage S2C_ConnectMessage { get; set; }
    }
}
