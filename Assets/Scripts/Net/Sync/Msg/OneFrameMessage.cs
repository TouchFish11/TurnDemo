using System.Collections.Generic;

namespace Net.Sync.Msg
{
    /// <summary>
    /// 存储客户端一帧内的所有操作消息
    /// </summary>
    public class OneFrameMessage : FrameMessage
    {
        /// <summary>
        /// ֡当前帧ID
        /// </summary>
        public int FrameID { get; set; }

        /// <summary>
        /// 当前帧ID（当前帧）包含的所有操作消息
        /// </summary>
        public List<OptMessage> OptMessages { get; set; }
        
        public override int GetMsgLength()
        {
            var length = 0;
            length += sizeof(int);    // 帧ID
            length += sizeof(int);    // 列表长度
            // 累加所有消息的长度
            foreach (var optMessage in OptMessages)
            {
                length += optMessage.GetMsgLength();
            }
            return length;
        }

        public override byte[] Serialize()
        {
            var index = 0;
            var bytes = new byte[GetMsgLength()];
            // 序列化帧ID
            MessageUtil.WriteField(bytes, FrameID, ref index);
            // 先写入列表长度
            MessageUtil.WriteField(bytes, OptMessages.Count, ref index);
            // 依次序列化所有操作消息
            foreach (var optMessage in OptMessages)
            {
                MessageUtil.WriteField(bytes, optMessage, ref index);
            }
            return bytes;
        }

        public override int Deserialize(byte[] bytes, int beginIndex = 0)
        {
            var index = beginIndex;
            // 反序列化帧ID
            FrameID = MessageUtil.ReadInt(bytes, ref index);
            // 先读取列表长度
            var count = MessageUtil.ReadInt(bytes, ref index);
            OptMessages = new List<OptMessage>();
            // 依次读取所有操作消息
            for (var i = 0; i < count; i++)
            {
                OptMessages.Add(MessageUtil.ReadFrameMessage<OptMessage>(bytes, ref index));
            }
            return index - beginIndex;
        }
    }
}
