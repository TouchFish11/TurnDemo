namespace Net.FrameSync.Tcp.Message.C2S
{
    /// <summary>
    /// �ͻ��˷��ͷ�����_����Ϣ_1001
    /// </summary>
    public class C2S_BindMessage : TcpMessage
    {
        /// <summary>
        /// �ͻ���Udp��̬�󶨶˿�
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
            return $"��ң�{ClientID}���Ѷ�̬�󶨿ͻ��˶˿ڣ�{UdpPort}";
        }
    }
}
