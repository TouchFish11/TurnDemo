using Core.Net.Tcp.Message.S2C;

namespace Core.EventCenter.Events
{
    /// <summary>
    /// ������ɺ��¼�
    /// </summary>
    public class PostConnectedEvent : Event
    {
        public S2C_ConnectMessage S2C_ConnectMessage { get; set; }
    }
}
