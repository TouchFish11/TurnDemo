using System.Collections.Generic;

namespace Net.FrameSync.Command
{
    /// <summary>
    /// 某一帧中所有客户端的帧命令
    /// ——包含一帧中所有客户端的所有帧操作
    /// </summary>
    public class OneFrameCommand : FrameCommand
    {
        /// <summary>
        /// 帧ID
        /// </summary>
        public int FrameID { get; set; }

        /// <summary>
        /// 存放所有客户端的帧命令
        /// </summary>
        public List<ClientFrameCommand> Commands { get; set; }

        public override int GetBytesLength()
        {
            int length = 0;
            // 帧ID
            length += 4;
            // 列表内容长度
            length += 4;
            // 列表内容
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
