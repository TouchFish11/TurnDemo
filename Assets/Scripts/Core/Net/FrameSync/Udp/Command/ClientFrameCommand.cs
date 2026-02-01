
namespace Core.Net.FrameSync.Udp.Command
{
    /// <summary>
    /// һ���ͻ���֡����
    /// ��������������Э����
    /// </summary>
    public class ClientFrameCommand : FrameCommand
    {
        /// <summary>
        /// �ͻ���ID
        /// </summary>
        public int ClientID { get; set; }

        /// <summary>
        /// ֡���������ͣ������ƶ��������������ͷ�
        /// </summary>
        public byte CommandType { get; set; }

        /// <summary>
        /// ֡�����Ĳ���һ
        /// </summary>
        public int Arg1 { get; set; }

        /// <summary>
        /// ֡�����Ĳ�����
        /// </summary>
        public int Arg2 { get; set; }

        /// <summary>
        /// ֡�����Ĳ�����
        /// </summary>
        public int Arg3 { get; set; }

        public override int GetBytesLength()
        {
            return 4 + 1 + 4 + 4 + 4;
        }

        public override byte[] Serialize()
        {
            int index = 0;
            byte[] bytes = new byte[GetBytesLength()];

            WriteField<int>(bytes, ClientID, ref index);
            WriteField<byte>(bytes, CommandType, ref index);
            WriteField<int>(bytes, Arg1, ref index);
            WriteField<int>(bytes, Arg2, ref index);
            WriteField<int>(bytes, Arg3, ref index);

            return bytes;
        }

        public override int Deserialize(byte[] bytes, int beginIndex = 0)
        {
            int index = beginIndex;

            // ��ȡ�ͻ���ID
            ClientID = ReadInt(bytes, ref index);
            // ��ȡ��������
            CommandType = ReadByte(bytes, ref index);
            // ��ȡ����һ
            Arg1 = ReadInt(bytes, ref index);
            // ��ȡ������
            Arg2 = ReadInt(bytes, ref index);
            // ��ȡ������
            Arg3 = ReadInt(bytes, ref index);

            return index - beginIndex;
        }
    }
}
