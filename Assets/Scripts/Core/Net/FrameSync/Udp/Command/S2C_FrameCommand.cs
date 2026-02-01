using System.Collections.Generic;

namespace Core.Net.FrameSync.Udp.Command
{
    /// <summary>
    /// 服务器发送给客户端的帧命令
    /// </summary>
    public class S2C_FrameCommand : FrameCommand
    {
        /// <summary>
        /// 当前帧ID
        /// </summary>
        public int FrameId { get; set; }

        /// <summary>
        /// 某几帧的指令列表
        /// </summary>
        public List<OneFrameCommand> Commands { get; set; }

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

            WriteField<int>(bytes, FrameId, ref index);
            WriteField<List<OneFrameCommand>>(bytes, Commands, ref index);

            return bytes;
        }

        public override int Deserialize(byte[] bytes, int beginIndex = 0)
        {
            int index = beginIndex;

            FrameId = ReadInt(bytes, ref index);
            Commands = ReadCommandList<OneFrameCommand>(bytes, ref index);

            return index - beginIndex;
        }
    }
}
