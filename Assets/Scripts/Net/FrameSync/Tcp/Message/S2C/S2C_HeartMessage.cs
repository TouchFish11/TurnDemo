namespace Net.FrameSync.Tcp.Message.S2C
{
    /// <summary>
    /// 服务器发送客户端_心跳消息_2000
    /// </summary>
    public class S2C_HeartMessage : TcpMessage
    {
        public override int GetMsgID()
        {
            return 2000;
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
