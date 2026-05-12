using System.Collections.Generic;

namespace Net.FrameSync.Tcp.Message.S2C
{
    /// <summary>
    /// ���������Ϳͻ���_������Ϣ_2001
    /// </summary>
    public class S2C_ConnectMessage : TcpMessage
    {
        /// <summary>
        /// ���������ӵĿͻ���ID
        /// </summary>
        public List<int> ClientIds {  get; set; }

        /// <summary>
        /// ����״̬
        /// true��ClientID��ʾ����Ŀͻ��ˣ�ClientIds��ʾ���������ӵĿͻ���ID
        /// false��ClientID��ʾ�Ͽ��Ŀͻ��ˣ�ClientIds����ʾ�κ�����
        /// </summary>
        public bool ConnectState { get; set; }

        public override int GetMsgID()
        {
            return 2001;
        }

        protected override int GetBytesBodyLength()
        {
            return 4 + 4 * ClientIds.Count + 1;
        }

        protected override void SerializeBody(byte[] bytes, ref int index)
        {
            WriteField<List<int>>(bytes, ClientIds, ref index);
            WriteField(bytes, ConnectState, ref index);
        }

        protected override void DeserializeBody(byte[] bytes, ref int index)
        {
            ClientIds = ReadListInt(bytes, ref index);
            ConnectState = ReadBool(bytes, ref index);
        }

        public override string ToString()
        {
            return $"��ң�{ClientID}��{(ConnectState ? "������" : "�Ͽ�����")}";
        }
    }
}
