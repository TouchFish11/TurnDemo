using Net.Protocols.Configs;
using Net.Protocols.Tcp;

namespace Core.Net.Protocols.Tcp.Messages.Common
{
    /// <summary>
    /// 新客户端加入消息，此时的SessionID是新客户端的ID，服务器下发给所有其它客户端
    /// <remarks>
    /// ID：<see cref="MessageIDConfig.S2C_ClientJoin_ID"/>
    /// </remarks>
    /// </summary>
    public class S2C_ClientJoinMessage : TcpMessage
    {
        public override int GetMsgID()
        {
            return MessageIDConfig.S2C_ClientJoin_ID;
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
