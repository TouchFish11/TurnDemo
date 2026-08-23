using Net.Protocols.Configs;
using Net.Protocols.Tcp;

namespace Core.Net.Protocols.Tcp.Messages.Common
{
    /// <summary>
    /// 断开连接消息。断开连接消息ID 客户端主动断开则发送给服务器作为断开请求； 客户端收到服务器则可以真正断开
    /// <remarks>
    /// ID：<see cref="MessageIDConfig.C2S_RequestDisconnect_ID"/>
    /// </remarks>
    /// </summary>
    public class C2S_RequestDisConnectMessage : TcpMessage
    {
        public override int GetMsgID()
        {
            return MessageIDConfig.C2S_RequestDisconnect_ID;
        }

        protected override int OnGetBodyLength()
        {
            return 0;
        }

        protected override void SerializeBody(byte[] bytes, ref int index)
        {

        }

        protected override void DeserializeBody(byte[] bytes, ref int index)
        {

        }
    }
}
