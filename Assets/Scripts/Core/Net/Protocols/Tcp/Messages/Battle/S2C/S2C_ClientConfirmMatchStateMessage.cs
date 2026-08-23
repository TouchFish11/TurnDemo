using Net.Protocols.Configs;

namespace Net.Protocols.Tcp.Messages.Battle.S2C
{
    /// <summary>
    /// 通知其它客户端某个客户端确认状态的消息
    /// </summary>
    public class S2C_ClientConfirmMatchStateMessage : TcpMessage
    {
        public bool CurrentConfirmMatchState { get; set; }
        
        public override int GetMsgID()
        {
            return MessageIDConfig.S2C_Notice_MatchConfirmState_ID;
        }

        protected override int OnGetBodyLength()
        {
            return sizeof(bool);
        }

        protected override void SerializeBody(byte[] bytes, ref int index)
        {
            MessageUtil.WriteField(bytes, CurrentConfirmMatchState, ref index);
        }

        protected override void DeserializeBody(byte[] bytes, ref int index)
        {
            CurrentConfirmMatchState = MessageUtil.ReadBool(bytes, ref index);
        }
    }
}
