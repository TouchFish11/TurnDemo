using System;
using Core.Log;
using kcp2k;
using Net.Protocols;

namespace Core.Net.SyncModule.Clients
{
    /// <summary>
    /// 对KCP2k客户端进行封装
    /// </summary>
    internal class KcpClient : IProtocolClient
    {
        // kcp2k
        private readonly kcp2k.KcpClient _kcp2kClient;
        // kcp配置
        private KcpConfig _kcp2kConfig;

        public bool IsConnected => _kcp2kClient.connected;
        
        public event Action<byte[], EProtocolChannel> OnDataReceived;
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<string> OnError;

        public KcpClient(KcpConfig kcp2kConfig)
        {
            _kcp2kClient = new kcp2k.KcpClient(
                OnConnect, 
                OnDataReceive,
                OnDisconnect,
                (code, msg) => OnError?.Invoke($"{code}_{msg}"),
                kcp2kConfig);
            _kcp2kConfig = kcp2kConfig;
        }

        public void Connect(string serverIp, ushort serverPort)
        {
            _kcp2kClient.Connect(serverIp, serverPort);
        }

        public void SendAsync(byte[] data, EProtocolChannel channel)
        {
            // 转换为kcp的通道
            var kcp2kChannel = channel == EProtocolChannel.Resolve ? KcpChannel.Reliable : KcpChannel.Unreliable;
            _kcp2kClient.Send(new ArraySegment<byte>(data), kcp2kChannel);
        }

        private void OnDataReceive(ArraySegment<byte> rawData, KcpChannel channel)
        {
            Logger.LogDebug(ELogTags.Network,$"[KcpClient] 收到数据包");
            // 直接返回原始数据给上层即可
            OnDataReceived?.Invoke(rawData.Array, channel == KcpChannel.Reliable ? EProtocolChannel.Resolve : EProtocolChannel.Raw);
        }

        private void OnConnect()
        {
            Logger.LogDebug(ELogTags.Network, $"[KcpClient] 连接成功!");
            OnConnected?.Invoke();
        }

        private void OnDisconnect()
        {
            Logger.LogDebug(ELogTags.Network, "[KcpClient] 断开连接");
            OnDisconnected?.Invoke();
        }

        public void Tick()
        {
            _kcp2kClient.Tick();
        }

        public void Disconnect()
        {
            _kcp2kClient.Disconnect();
        }
    }
}
