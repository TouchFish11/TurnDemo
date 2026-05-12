using System;
using Core.DI;
using Core.Log;
using Net.Sync.Msg.S2C;

namespace Net.Sync
{
    /// <summary>
    /// 网络游戏代理
    /// </summary>
    public class NetGameProxy : INetGameProxy
    {
        // 封装底层网络管理器
        private INetManager _netManager;
        // 消息路由处理
        private MessageRouter _router;
        // 游戏层关注的事件
        public event Action<int> OnGameConnected;
        public event Action OnGameDisconnected;
        
        /// 服务器下发的当前连接的客户端Token
        public int SessionId { get; private set; }
        
        private NetGameProxy(){}
        
        public INetGameProxy Init(NetConfig netConfig)
        {
            _netManager = DIContainer.Create<NetManager>(parameterValues: netConfig);
            _netManager.OnConnected += OnGameConnectedInternal;
            _netManager.OnMessageReceived += OnMessageReceive;
            _router = DIContainer.Create<MessageRouter>();
            return this;
        }
    
        // 游戏层调用接口
        public void Connect()
        {
            _netManager.Connect();
        }

        public void SetSessionToken(int sessionToken)
        {
            OnGameConnected?.Invoke(sessionToken);
            SessionId = sessionToken;
        }

        public void Send(Message message, EProtocolChannel channel)
        {
            message.SessionID = SessionId;
            _netManager.Send(message, channel);
        }

        public void Disconnect()
        {
            _netManager.Disconnect();
        }

        private void OnGameConnectedInternal()
        {
            Logger.Log($"[Net Connected] 连接服务器成功!");
        }

        private void OnMessageReceive(Message message, EProtocolChannel channel)
        {
            if (message == null) return;
            _router.Dispatch(message);
        }
    }
}
