namespace Net.FrameSync.Tcp.Message.C2S
{
    /// <summary>
    /// �ͻ��˷��ͷ�����_����ȷ����Ϣ_1008
    /// </summary>
    public class C2S_ConnectConfirmMessage : TcpMessage
    {
        public override int GetMsgID()
        {
            return 1008;
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
