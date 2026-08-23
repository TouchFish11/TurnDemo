
using Net.Protocols.Configs;

namespace Net.Protocols.Tcp.Messages.Battle.S2C
{
    /// <summary>
    /// 匹配成功消息
    /// <remarks>
    /// ID：<see cref="MessageIDConfig.S2C_MatchSuccess_ID"/>
    /// </remarks>
    /// </summary>
    public class S2C_MatchSuccessMessage : TcpMessage
    {
        /// <summary>
        /// 比赛人数
        /// </summary>
        public byte MatchPlayerCount { get; set; }

        public override int GetMsgID()
        {
            return MessageIDConfig.S2C_MatchSuccess_ID;
        }

        protected override int OnGetBodyLength()
        {
            return 1;
        }
        
        protected override void SerializeBody(byte[] bytes, ref int index)
        {
            MessageUtil.WriteField(bytes, MatchPlayerCount, ref index);
        }

        protected override void DeserializeBody(byte[] bytes, ref int index)
        {
            MatchPlayerCount = MessageUtil.ReadByte(bytes, ref index);
        }
    }
}
