namespace Net.Sync
{
    /// <summary>
    /// 统一的消息基类
    /// </summary>
    public abstract class Message
    {
        /// <summary>
        /// 发送消息的客户端会话ID，由服务器下发
        /// </summary>
        public int SessionID {  get; set; }
        
        /// <summary>
        /// 获取消息总长度
        /// </summary>
        /// <returns></returns>
        public abstract int GetMsgLength();
        
        /// <summary>
        /// 序列化消息，消息自定义序列化规则
        /// </summary>
        /// <returns></returns>
        public abstract byte[] Serialize();

        /// <summary>
        /// 反序列化字节数组为消息，消息自定义反序列化规则，要与序列化规则匹配
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="beginIndex"></param>
        /// <returns></returns>
        public abstract int Deserialize(byte[] bytes, int beginIndex = 0);
    }
}
