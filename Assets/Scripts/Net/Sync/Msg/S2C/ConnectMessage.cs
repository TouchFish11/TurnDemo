using System.Collections.Generic;

namespace Net.Sync.Msg.S2C
{
    /// <summary>
    /// 连接消息
    /// 客户端发送给服务器时，无需设置任何字段
    /// 服务器发送给客户端时
    /// </summary>
    public class ConnectMessage : Message
    {
        /// <summary>
        /// 所有已连接的客户端ID，包括发送目标客户端
        /// </summary>
        public List<int> ClientIds { get; set; }

        /// <summary>
        /// 连接状态
        /// true：ClientID表示连入的客户端，ClientIds表示其它已连接的客户端ID
        /// false：ClientID表示断开的客户端，ClientIds不表示任何内容
        /// </summary>
        public bool ConnectState { get; set; }

        public override int GetMsgLength()
        {
            return sizeof(int) +                        // 消息ID
                   sizeof(int) * ClientIds.Count +      // ClientIds
                   sizeof(bool);                        // ConnectState
        }

        public override byte[] Serialize()
        {
            var index = 0;
            var bytes = new byte[GetMsgLength()];
            
            // 序列化客户端列表
            MessageUtil.WriteField(bytes, ClientIds.Count, ref index);
            foreach (var clientId in ClientIds)
            {
                MessageUtil.WriteField(bytes, clientId, ref index);
            }
            // 序列化连接状态
            MessageUtil.WriteField(bytes, ConnectState, ref index);
            return bytes;
        }

        public override int Deserialize(byte[] bytes, int beginIndex = 0)
        {
            var nowIndex = beginIndex;
            ClientIds = new List<int>();
            var count = MessageUtil.ReadInt(bytes, ref nowIndex);
            for (var i = 0; i < count; i++)
            {
                ClientIds.Add(MessageUtil.ReadInt(bytes, ref nowIndex));
            }
            ConnectState = MessageUtil.ReadBool(bytes, ref nowIndex);
            return nowIndex - beginIndex;
        }
    }
}
