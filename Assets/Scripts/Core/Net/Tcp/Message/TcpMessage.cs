namespace Core.Net.Tcp.Message
{
    /// <summary>
    /// TCP��Ϣ
    /// </summary>
    public abstract class TcpMessage : BaseMessage
    {
        /// <summary>
        /// �ͻ���ID
        /// </summary>
        public int ClientID {  get; set; }

        /// <summary>
        /// ��ȡ��ϢID
        /// </summary>
        /// <returns></returns>
        public abstract int GetMsgID();

        /// <summary>
        /// ��ȡ�ֽ����鳤��
        /// </summary>
        /// <returns></returns>
        public override int GetBytesLength()
        {
            //��ϢID + ���峤 + �ͻ���ID + ����
            return 4 + 4 + 4 + GetBytesBodyLength();
        }

        /// <summary>
        /// ��ȡ�ֽ���������峤�ȣ�������֡ID����ϢID��
        /// </summary>
        /// <returns></returns>
        protected abstract int GetBytesBodyLength();

        /// <summary>
        /// TCP��Ϣ���л�
        /// </summary>
        /// <returns></returns>
        public override byte[] Serialize()
        {
            int index = 0;
            byte[] bytes = new byte[GetBytesLength()];

            //д����ϢID
            WriteField<int>(bytes, GetMsgID(), ref index);
            //д�����峤��
            WriteField<int>(bytes, GetBytesBodyLength(), ref index);
            //д��ͻ���ID
            WriteField<int>(bytes, ClientID, ref index);
            //д���Զ����ֶ�
            SerializeBody(bytes, ref index);
            return bytes;
        }

        /// <summary>
        /// ���л���������
        /// </summary>
        /// <returns></returns>
        protected abstract void SerializeBody(byte[] bytes, ref int index);

        /// <summary>
        /// �����л�
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="beginIndex"></param>
        /// <returns></returns>
        public override int Deserialize(byte[] bytes, int beginIndex = 0)
        {
            int index = beginIndex;
            //��ϢID�����峤���ⲿ�Ѵ���
            //��ȡ�ͻ���ID
            ClientID = ReadInt(bytes, ref index);
            //��ȡ�Զ�������
            DeserializeBody(bytes, ref index);
            return index - beginIndex;
        }

        /// <summary>
        /// �����л���������
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        protected abstract void DeserializeBody(byte[] bytes, ref int index);

    }
}
