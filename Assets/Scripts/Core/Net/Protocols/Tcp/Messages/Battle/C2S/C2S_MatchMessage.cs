using Net.Protocols;
using Net.Protocols.Configs;
using Net.Protocols.Tcp;

namespace Core.Net.Protocols.Tcp.Messages.Battle.C2S
{
    /// <summary>
    /// 请求匹配消息，需设置客户端ID
    /// </summary>
    public class C2S_MatchMessage : TcpMessage
    {
        /// <summary>
        /// 匹配请求，true为请求匹配，false为取消当前匹配
        /// </summary>
        public bool MatchRequest { get; set; }
        
        public override int GetMsgID()
        {
            return MessageIDConfig.C2S_Match_ID;
        }

        protected override int OnGetBodyLength()
        {
            return 1;
        }

        protected override void SerializeBody(byte[] bytes, ref int index)
        {
            MessageUtil.WriteField(bytes, MatchRequest, ref index);
        }

        protected override void DeserializeBody(byte[] bytes, ref int index)
        {
            MatchRequest = MessageUtil.ReadBool(bytes, ref index);
        }
    }
}
