namespace Net.Sync.Msg.Chat
{
    /// <summary>
    /// 聊天消息
    /// </summary>
    public class ChatMessage : Message
    {
        /// <summary>
        /// 目标会话ID
        /// </summary>
        public int TargetSessionID { get; set; }

        /// <summary>
        /// 聊天消息
        /// </summary>
        public string ChatMsg { get; set; }
        
        public override int GetMsgLength()
        {
            return sizeof(int) +                 // 目标会话ID
                   sizeof(char) * ChatMsg.Length;   // 聊天消息长度
        }

        public override byte[] Serialize()
        {
            var index = 0;
            var bytes = new byte[GetMsgLength()];
            MessageUtil.WriteField(bytes, TargetSessionID, ref index);
            MessageUtil.WriteField(bytes, ChatMsg, ref index);
            return bytes;
        }

        public override int Deserialize(byte[] bytes, int beginIndex = 0)
        {
            var index = beginIndex;
            TargetSessionID = MessageUtil.ReadInt(bytes, ref index);
            ChatMsg = MessageUtil.ReadString(bytes, ref index);
            return index - beginIndex;
        }
    }
}
