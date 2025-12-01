
namespace Net.FrameSync.Command
{
    /// <summary>
    /// 一个客户端帧命令
    /// ——包含在其它协议中
    /// </summary>
    public class ClientFrameCommand : FrameCommand
    {
        /// <summary>
        /// 客户端ID
        /// </summary>
        public int ClientID { get; set; }

        /// <summary>
        /// 帧操作的类型，比如移动、攻击、技能释放
        /// </summary>
        public byte CommandType { get; set; }

        /// <summary>
        /// 帧操作的参数一
        /// </summary>
        public int Arg1 { get; set; }

        /// <summary>
        /// 帧操作的参数二
        /// </summary>
        public int Arg2 { get; set; }

        /// <summary>
        /// 帧操作的参数三
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

            // 读取客户端ID
            ClientID = ReadInt(bytes, ref index);
            // 读取操作类型
            CommandType = ReadByte(bytes, ref index);
            // 读取参数一
            Arg1 = ReadInt(bytes, ref index);
            // 读取参数二
            Arg2 = ReadInt(bytes, ref index);
            // 读取参数三
            Arg3 = ReadInt(bytes, ref index);

            return index - beginIndex;
        }
    }
}
