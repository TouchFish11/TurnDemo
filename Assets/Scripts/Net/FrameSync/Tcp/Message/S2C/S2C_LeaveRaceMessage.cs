namespace Net.FrameSync.Tcp.Message.S2C
{
    /// <summary>
    /// 服务器到客户端_离开比赛_2006
    /// </summary>
    public class S2C_LeaveRaceMessage : TcpMessage
    {
        public override int GetMsgID()
        {
            return 2006;
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
