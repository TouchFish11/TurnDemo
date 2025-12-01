namespace Net.TCP.Message.C2S
{
    /// <summary>
    /// 客户端发送服务器_绑定消息_1001
    /// </summary>
    public class C2S_BindMessage : TcpMessage
    {
        /// <summary>
        /// 客户端Udp动态绑定端口
        /// </summary>
        public int UdpPort { get; set; }

        public override int GetMsgID()
        {
            return 1001;
        }

        protected override int GetBytesBodyLength()
        {
            return 4;
        }

        protected override void SerializeBody(byte[] bytes, ref int index)
        {
            WriteField(bytes, UdpPort, ref index);
        }

        protected override void DeserializeBody(byte[] bytes, ref int index)
        {
            UdpPort = ReadInt(bytes, ref index);
        }

        public override string ToString()
        {
            return $"玩家：{ClientID}，已动态绑定客户端端口：{UdpPort}";
        }
    }
}
