using Net.Protocols;
using Net.Protocols.Configs;
using Net.Protocols.Tcp;

namespace Core.Net.Protocols.Tcp.Messages.Battle.C2S
{
    /// <summary>
    /// 匹配确认消息
    /// </summary>
    public class C2S_MatchConfirmMessage : TcpMessage
    {
        /// <summary>
        /// 是否匹配，true为接收，false为拒绝
        /// </summary>
        public bool IsMatch { get; set; }
        
        public override int GetMsgID()
        {
            return MessageIDConfig.C2S_MatchConfirm_ID;
        }

        protected override int OnGetBodyLength()
        {
            return 1;
        }

        protected override void SerializeBody(byte[] bytes, ref int index)
        {
            MessageUtil.WriteField(bytes, IsMatch, ref index);
        }

        protected override void DeserializeBody(byte[] bytes, ref int index)
        {
            IsMatch = MessageUtil.ReadBool(bytes, ref index);
        }
    }
}
