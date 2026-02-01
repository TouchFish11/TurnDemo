using System;

namespace Core.Net.Tcp.Message.C2S
{
    /// <summary>
    /// �ͻ��˷��ͷ�����_������Ϣ_1000
    /// </summary>
    public class C2S_HeartMessage : TcpMessage
    {
        public override int GetMsgID()
        {
            return 1000;
        }

        protected override int GetBytesBodyLength()
        {
            return 0;
        }

        protected override void SerializeBody(byte[] bytes, ref int index) { }

        protected override void DeserializeBody(byte[] bytes, ref int index) { }

        public override string ToString()
        {
            return $"��ң�{ClientID}��������Ϣ����{DateTime.Now}";
        }
    }
}
