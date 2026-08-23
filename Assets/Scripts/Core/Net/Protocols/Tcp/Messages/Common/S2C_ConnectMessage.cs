using System.Collections.Generic;
using Net.Protocols;
using Net.Protocols.Configs;
using Net.Protocols.Tcp;

namespace Core.Net.Protocols.Tcp.Messages.Common
{
    /// <summary>
    /// 连接消息
    /// <remarks>
    /// 服务器发送给所有客户端，需要设置<see cref="TcpMessage.SessionID"/>，并 携带其它客户端的ID<see cref="ClientIds"/>。
    /// </remarks>
    /// </summary>
    public class S2C_ConnectMessage : TcpMessage
    {
        /// <summary>
        /// 所有已连接的客户端ID，包括发送目标客户端
        /// </summary>
        public List<int> ClientIds { get; set; }

        public override int GetMsgID()
        {
            return MessageIDConfig.S2C_Connect_ID;
        }
        
        protected override int OnGetBodyLength()
        {
            return sizeof(int) + sizeof(int) * ClientIds.Count;    // ClientIds
        }

        protected override void SerializeBody(byte[] bytes, ref int index)
        {
            // 序列化客户端列表
            MessageUtil.WriteField(bytes, ClientIds.Count, ref index);
            foreach (var clientId in ClientIds)
            {
                MessageUtil.WriteField(bytes, clientId, ref index);
            }
        }

        protected override void DeserializeBody(byte[] bytes, ref int index)
        {
            var count = MessageUtil.ReadInt(bytes, ref index);
            ClientIds = new List<int>();
            for (var i = 0; i < count; i++)
            {
                ClientIds.Add(MessageUtil.ReadInt(bytes, ref index));
            }
        }
    }
}
