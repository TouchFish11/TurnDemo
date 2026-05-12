namespace Net.FrameSync.Tcp.Message.C2S
{
    /// <summary>
    /// �ͻ��˵�������_����������Ϣ_1007
    /// </summary>
    public class C2S_ReconnecRaceMessage : TcpMessage
    {
        public override int GetMsgID()
        {
            return 1007;
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
