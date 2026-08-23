using System;
using Core.DI;
using Core.Net.SyncModule.Interface;
using Net.Protocols;
using Net.Protocols.FSync.Messages;
using Net.Protocols.Tcp;

namespace Core.Net.SyncModule.Manager
{
    /// <summary>
    /// 网络游戏代理
    /// </summary>
    public class NetGameProxy : INetGameProxy
    {
        // 封装底层网络管理器
        [Inject] private INetManager _netManager;
        
        // 消息路由处理
        private MessageRouter _router;

        public int ClientId => _netManager.SessionId;
        
        public event Action<int, int[]> OnConnected;
        
        public event Action OnDisconnected;

        public event Action<long> TcpRtt;
        
        private NetGameProxy()
        {
            
        }
        
        public INetGameProxy Init(NetConfig netConfig)
        {
            _netManager = DIContainer.Resolve<NetManager>();
            _netManager.Init(netConfig);
            _netManager.OnConnected += OnConnectedEvent;
            _netManager.OnMessageReceived += OnMessageReceive;
            _netManager.OnDisconnected += OnDisconnectedEvent;
            _netManager.OnError += OnError;
            ((IHeartbeatService)_netManager).OnRttCalc += RttCalcEvent;
            _router = DIContainer.Create<MessageRouter>();
            return this;
        }
    
        // 游戏层调用接口
        public void Connect()
        {
            _netManager.Connect();
        }
        
        public void Send(Message message, EProtocolChannel channel)
        {
            if(channel == EProtocolChannel.Resolve)
                ((TcpMessage)message).SessionID = _netManager.SessionId;
            else
                ((C2S_NextFrameMessage)message).OptMessage.SessionID = _netManager.SessionId;
            _netManager.Send(message, channel);
        }

        public void Disconnect()
        {
            _netManager.Disconnect();
        }

        private void OnConnectedEvent(int clientId, int[] clientIds)
        {
            OnConnected?.Invoke(clientId, clientIds);
        }

        private void OnDisconnectedEvent()
        {
            OnDisconnected?.Invoke();
        }

        private void OnMessageReceive(Message message, EProtocolChannel channel)
        {
            _router.Dispatch(message);
        }

        private void RttCalcEvent(long rttMs)
        {
            TcpRtt?.Invoke(rttMs);
        }
        
        private void OnError(string message)
        {
            
        }
    }
}
