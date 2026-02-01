using System.Collections.Generic;

namespace Core.Net.FrameSync.Udp.Command
{
    /// <summary>
    /// ĳһ֡�����пͻ��˵�֡����
    /// ��������һ֡�����пͻ��˵�����֡����
    /// </summary>
    public class OneFrameCommand : FrameCommand
    {
        /// <summary>
        /// ֡ID
        /// </summary>
        public int FrameID { get; set; }

        /// <summary>
        /// ������пͻ��˵�֡����
        /// </summary>
        public List<ClientFrameCommand> Commands { get; set; }

        public override int GetBytesLength()
        {
            int length = 0;
            // ֡ID
            length += 4;
            // �б����ݳ���
            length += 4;
            // �б�����
            for (int i = 0; i < Commands.Count; i++)
            {
                length += Commands[i].GetBytesLength();
            }

            return length;
        }

        public override byte[] Serialize()
        {
            int index = 0;
            byte[] bytes = new byte[GetBytesLength()];

            WriteField<int>(bytes, FrameID, ref index);
            WriteField<List<ClientFrameCommand>>(bytes, Commands, ref index);

            return bytes;
        }

        public override int Deserialize(byte[] bytes, int beginIndex = 0)
        {
            int index = beginIndex;

            FrameID = ReadInt(bytes, ref index);
            Commands = ReadCommandList<ClientFrameCommand>(bytes, ref index);

            return index - beginIndex;
        }
    }
}
