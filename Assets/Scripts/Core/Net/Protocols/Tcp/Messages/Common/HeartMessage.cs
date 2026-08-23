using Net.Protocols;
using Net.Protocols.Configs;
using Net.Protocols.Tcp;

namespace Core.Net.Protocols.Tcp.Messages.Common
{
    /// <summary>
    /// 心跳消息
    /// </summary>
    public class HeartMessage : TcpMessage
    {
        /// <summary>
        /// 服务器时间戳
        /// </summary>
        public long ServerTimeStamp { get; set; }
        
        /// <summary>
        /// 客户端时间戳
        /// </summary>
        public long ClientTimeStamp { get; set; }
        
        public override int GetMsgID()
        {
            return MessageIDConfig.Heartbeat_ID;
        }
        
        protected override int OnGetBodyLength()
        {
            return sizeof(long) + sizeof(long);
        }

        protected override void SerializeBody(byte[] bytes, ref int index)
        {
            MessageUtil.WriteField(bytes, ServerTimeStamp, ref index);
            MessageUtil.WriteField(bytes, ClientTimeStamp, ref index);
        }
        
        protected override void DeserializeBody(byte[] bytes, ref int index)
        {
            ServerTimeStamp = MessageUtil.ReadLong(bytes, ref index);
            ClientTimeStamp = MessageUtil.ReadLong(bytes, ref index);
        }
    }
}
