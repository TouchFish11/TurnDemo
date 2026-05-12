namespace Net.Sync.Msg
{
    /// <summary>
    /// 帧同步的帧操作消息
    /// </summary>
    public class OptMessage : FrameMessage
    {
        /// <summary>
        /// 操作类型，不同字节数代表不同的游戏操作
        /// </summary>
        public byte OptType { get; set; }

        /// <summary>
        /// 通用参数1
        /// </summary>
        public int Arg1 { get; set; }

        /// <summary>
        /// 通用参数2
        /// </summary>
        public int Arg2 { get; set; }

        /// <summary>
        /// 通用参数3
        /// </summary>
        public int Arg3 { get; set; }
        
        public override int GetMsgLength()
        {
            return sizeof(int) +    //客户端ID
                   sizeof(byte) +   // 操作类型
                   sizeof(int) +    // 通用参数1
                   sizeof(int) +    // 通用参数2
                   sizeof(int);     // 通用参数3
        }

        public override byte[] Serialize()
        {
            var index = 0;
            var bytes = new byte[GetMsgLength()];
            // 序列化所有字段
            MessageUtil.WriteField(bytes, SessionID, ref index);
            MessageUtil.WriteField(bytes, OptType, ref index);
            MessageUtil.WriteField(bytes, Arg1, ref index);
            MessageUtil.WriteField(bytes, Arg2, ref index);
            MessageUtil.WriteField(bytes, Arg3, ref index);
            return bytes;
        }

        public override int Deserialize(byte[] bytes, int beginIndex = 0)
        {
            var index = beginIndex;
            // 赋值所有字段
            SessionID = MessageUtil.ReadInt(bytes, ref index);
            OptType = MessageUtil.ReadByte(bytes, ref index);
            Arg1 = MessageUtil.ReadInt(bytes, ref index);
            Arg2 = MessageUtil.ReadInt(bytes, ref index);
            Arg3 = MessageUtil.ReadInt(bytes, ref index);
            return index - beginIndex;
        }
    }
}
