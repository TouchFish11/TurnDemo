namespace Net.Protocols.Tcp
{
    /// <summary>
    /// TCP消息
    /// </summary>
    public abstract class TcpMessage : Message
    {
        /// <summary>
        /// 发送消息的客户端唯一ID，由服务器下发
        /// </summary>
        public int SessionID { get; set; }
        
        public abstract int GetMsgID();
        
        public sealed override int GetMsgLength()
        {
            // 消息ID + 消息体长度 + 消息体
            return 4 + 4 + GetBodyLength();
        }

        /// <summary>
        /// 消息体长度，不包含消息头
        /// </summary>
        /// <returns></returns>
        private int GetBodyLength()
        {
            // 包含客户端ID
            return sizeof(int) + OnGetBodyLength();
        }
        
        /// <summary>
        /// 子类消息体长度，不包含消息头和客户端ID
        /// </summary>
        /// <returns></returns>
        protected abstract int OnGetBodyLength();

        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        public sealed override byte[] Serialize()
        {
            var index = 0;
            var bytes = new byte[GetMsgLength()];

            // 序列化消息ID
            MessageUtil.WriteField(bytes, GetMsgID(), ref index);
            // 序列化消息体长度
            MessageUtil.WriteField(bytes, GetBodyLength(), ref index);
            // 序列化客户端ID
            MessageUtil.WriteField(bytes, SessionID, ref index);
            // 序列化消息体
            SerializeBody(bytes, ref index);
            return bytes;
        }

        /// <summary>
        /// 序列化消息体
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        protected abstract void SerializeBody(byte[] bytes, ref int index);

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="beginIndex"></param>
        /// <returns></returns>
        public sealed override int Deserialize(byte[] bytes, int beginIndex = 0)
        {
            var index = beginIndex;
            SessionID = MessageUtil.ReadInt(bytes, ref index);
            DeserializeBody(bytes, ref index);
            return index - beginIndex;
        }

        /// <summary>
        /// 反序列化消息体
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        protected abstract void DeserializeBody(byte[] bytes, ref int index);
    }
}
