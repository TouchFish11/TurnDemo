using System;
using Core.DI;
using Core.Mono;
using kcp2k;

namespace Net.Sync
{
    /// <summary>
    /// 网络管理器
    /// </summary>
    public class NetManager : INetManager
    {
        /// <summary>
        /// 默认配置
        /// </summary>
        private static readonly NetConfig DefaultConfig = new()
        {
            Serializer = MessageSerializerGetter.BinaryMessageSerializer(),
            ClientType = EClientType.Kcp,
            KcpConfig = new KcpConfig()
        };
        
        // 客户端
        private readonly IProtocolClient _client;
        // 消息序列化器
        private readonly IMessageSerializer _messageSerializer;
        // 当前网络配置
        private readonly NetConfig _config;
        
        public event Action<Message, EProtocolChannel> OnMessageReceived;
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<string> OnError;
        
        public NetManager(NetConfig config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            
            // 初始化
            _messageSerializer = config.Serializer ?? DefaultConfig.Serializer;
            _client = config.ClientType switch
            {
                EClientType.Dual => new DualChannelClient(),
                EClientType.Kcp => new KcpClient(config.KcpConfig ?? DefaultConfig.KcpConfig),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            _client.OnConnected += () => OnConnected?.Invoke();
            _client.OnDisconnected += () => OnDisconnected?.Invoke();
            _client.OnError +=  error => OnError?.Invoke(error);
            _client.OnDataReceived += (messageData, channel) => OnMessageReceived?.Invoke(_messageSerializer.Deserialize(messageData, channel), channel);
            _config = config;
            
            DIContainer.GetInstance<IMonoAdapter>().AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// 连接到指定IP和端口的服务器
        /// </summary>
        public void Connect()
        {
            _client.Connect(_config.ServerIp, _config.ServerPort);
        }

        public void Send(Message message, EProtocolChannel channel)
        {
            var messageBytes = _messageSerializer.Serialize(message, channel);
            _client.Send(messageBytes, channel);
        }

        public void OnUpdate()
        {
            _client.Tick();
        }

        public void Disconnect()
        {
            _client.Disconnect();
            DIContainer.GetInstance<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }
    }
}
