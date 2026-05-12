using kcp2k;

namespace Net.Sync
{
    public class NetConfig
    {
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIp { get; set; }
        
        /// <summary>
        /// 服务器端口
        /// </summary>
        public ushort ServerPort{get; set; }
        
        /// <summary>
        /// 消息序列化器
        /// </summary>
        public IMessageSerializer Serializer { get; set; }
        
        /// <summary>
        /// 协议类型
        /// </summary>
        public EClientType ClientType { get; set; }
        
        /// <summary>
        /// 专属Kcp配置，其它协议忽略此属性，当协议类型为Kcp时使用此属性，若为空则使用默认的kcp配置
        /// </summary>
        public KcpConfig KcpConfig { get; set; }
    }
}
