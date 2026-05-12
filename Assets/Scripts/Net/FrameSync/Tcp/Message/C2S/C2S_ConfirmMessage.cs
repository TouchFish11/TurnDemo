namespace Net.FrameSync.Tcp.Message.C2S
{
    /// <summary>
    /// �ͻ��˷��ͷ�����_ȷ����Ϣ_1004
    /// </summary>
    public class C2S_ConfirmMessage : TcpMessage
    {
        public override int GetMsgID()
        {
            return 1004;
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
