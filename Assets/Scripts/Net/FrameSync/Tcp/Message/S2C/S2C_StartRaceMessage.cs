namespace Net.FrameSync.Tcp.Message.S2C
{
    /// <summary>
    /// 服务器发送客户端_开始比赛消息_2005
    /// </summary>
    public class S2C_StartRaceMessage : TcpMessage
    {
        public override int GetMsgID()
        {
            return 2005;
        }

        protected override int GetBytesBodyLength()
        {
            return 0;
        }

        protected override void SerializeBody(byte[] bytes, ref int index)
        {

        }

        protected override void DeserializeBody(byte[] bytes, ref int index)
        {

        }
    }
}
