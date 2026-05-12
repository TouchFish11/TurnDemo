
namespace Net.FrameSync.Command
{
    /// <summary>
    /// 客户端发送给服务器的下一帧指令
    /// </summary>
    public class C2S_NextFrameCommand : FrameCommand
    {
        /// <summary>
        /// 帧ID
        /// </summary>
        public int FrameId { get; set; }

        /// <summary>
        /// 客户端的指令
        /// </summary>
        public ClientFrameCommand ClientFrameCommand { get; set; }

        public override int GetBytesLength()
        {
            return 4 + ClientFrameCommand.GetBytesLength();
        }

        public override byte[] Serialize()
        {
            int index = 0;
            byte[] bytes = new byte[GetBytesLength()];

            WriteField<int>(bytes, FrameId, ref index);
            WriteField<ClientFrameCommand>(bytes, ClientFrameCommand, ref index);

            return bytes;
        }

        public override int Deserialize(byte[] bytes, int beginIndex = 0)
        {
            int index = beginIndex;

            FrameId = ReadInt(bytes, ref index);
            ClientFrameCommand = ReadFrameCommand<ClientFrameCommand>(bytes, ref index);

            return beginIndex - index;
        }
    }
}
