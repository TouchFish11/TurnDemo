
namespace Net.FrameSync.Tcp.Message.S2C
{
    /// <summary>
    /// 服务器到客户端_匹配成功消息_2002
    /// </summary>
    public class S2C_MatchSuccessMessage : TcpMessage
    {
        /// <summary>
        /// 比赛人数
        /// </summary>
        public byte MatchPlayerCount {  get; set; }

        public override int GetMsgID()
        {
            return 2002;
        }

        protected override int GetBytesBodyLength()
        {
            return 1;
        }

        protected override void DeserializeBody(byte[] bytes, ref int index)
        {
            MatchPlayerCount = ReadByte(bytes, ref index);
        }

        protected override void SerializeBody(byte[] bytes, ref int index)
        {
            WriteField<byte>(bytes, MatchPlayerCount, ref index);
        }
    }
}
