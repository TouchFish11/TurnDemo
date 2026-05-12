namespace Net.FrameSync.Tcp.Message.C2S
{
    /// <summary>
    /// �ͻ��˷��ͷ�����_ƥ����Ϣ_1003
    /// </summary>
    public class C2S_MatchMessage : TcpMessage
    {
        /// <summary>
        /// �Ƿ�ƥ��
        /// TrueΪ����ƥ�䣻falseΪȡ��ƥ��
        /// </summary>
        public bool IsMatch { get; set; }

        public override int GetMsgID()
        {
            return 1003;
        }

        protected override int GetBytesBodyLength()
        {
            return 1;
        }

        protected override void SerializeBody(byte[] bytes, ref int index)
        {
            WriteField<bool>(bytes, IsMatch, ref index);
        }

        protected override void DeserializeBody(byte[] bytes, ref int index)
        {
            IsMatch = ReadBool(bytes, ref index);
        }
    }
}
