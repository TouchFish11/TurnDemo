namespace Net.FrameSync.Tcp.Message.C2S
{
    /// <summary>
    /// �ͻ��˷��ͷ�����_׼��������Ϣ_1005
    /// </summary>
    public class C2S_ReadyMessage : TcpMessage
    {
        public override int GetMsgID()
        {
            return 1005;
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
