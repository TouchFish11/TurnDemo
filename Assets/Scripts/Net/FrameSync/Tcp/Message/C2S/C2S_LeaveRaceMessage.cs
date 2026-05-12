namespace Net.FrameSync.Tcp.Message.C2S
{
    /// <summary>
    /// �ͻ��˷��ͷ�����_�뿪������Ϣ_1006
    /// </summary>
    public class C2S_LeaveRaceMessage : TcpMessage
    {
        public override int GetMsgID()
        {
            return 1006;
        }

        protected override int GetBytesBodyLength()
        {
            return 0;
        }

        protected override void DeserializeBody(byte[] bytes, ref int index)
        {

        }

        protected override void SerializeBody(byte[] bytes, ref int index)
        {

        }
    }
}
