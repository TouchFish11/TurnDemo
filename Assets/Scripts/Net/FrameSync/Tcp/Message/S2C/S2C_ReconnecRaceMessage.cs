namespace Net.FrameSync.Tcp.Message.S2C
{
    /// <summary>
    /// 服务器到客户端_重连比赛消息_2007
    /// </summary>
    public class S2C_ReconnecRaceMessage : TcpMessage
    {
        public override int GetMsgID()
        {
            return 2007;
        }

        protected override void DeserializeBody(byte[] bytes, ref int index)
        {

        }

        protected override int GetBytesBodyLength()
        {
            return 0;
        }

        protected override void SerializeBody(byte[] bytes, ref int index)
        {

        }
    }
}
