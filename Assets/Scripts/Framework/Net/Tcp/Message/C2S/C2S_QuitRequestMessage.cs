namespace Net.TCP.Message.C2S
{
    /// <summary>
    /// 客户端发送服务器_退出请求消息_1002
    /// </summary>
    public class C2S_QuitRequestMessage : TcpMessage
    {
        public override int GetMsgID()
        {
            return 1002;
        }

        protected override int GetBytesBodyLength()
        {
            return 0;
        }

        protected override void SerializeBody(byte[] bytes, ref int index) { }

        protected override void DeserializeBody(byte[] bytes, ref int index) { }

        public override string ToString()
        {
            return $"玩家：{ClientID}，请求退出连接";
        }
    }
}
