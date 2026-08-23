using Net.Protocols.Configs;
using Net.Protocols.Tcp;

namespace Core.Net.Protocols.Tcp.Messages.Common
{
    /// <summary>
    /// 断开连接消息
    /// </summary>
    public class S2C_DisConnectMessage : TcpMessage
    {
        public override int GetMsgID()
        {
            return MessageIDConfig.S2C_Disconnect_ID;
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
