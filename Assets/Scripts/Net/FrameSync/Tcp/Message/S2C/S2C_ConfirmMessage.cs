namespace Net.FrameSync.Tcp.Message.S2C
{
    /// <summary>
    /// 服务器到客户端_确认消息_2003
    /// </summary>
    public class S2C_ConfirmMessage : TcpMessage
    {
        public override int GetMsgID()
        {
            return 2003;
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
