using System;
using Core.Log;
using kcp2k;

namespace Net.Sync
{
    /// <summary>
    /// 对KCP2k客户端进行封装
    /// </summary>
    internal class KcpClient : IProtocolClient
    {
        // kcp2k
        private readonly kcp2k.KcpClient _kcp2kClient;
        private KcpConfig _kcp2kConfig;
        
        public event Action<byte[], EProtocolChannel> OnDataReceived;
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<string> OnError;

        public KcpClient(KcpConfig kcp2kConfig)
        {
            _kcp2kClient = new kcp2k.KcpClient(
                () =>
                {
                    Logger.LogDebug(TODO, $"[KcpClient] 连接成功!");
                    OnConnected?.Invoke();
                },
                (data, kcp2kChannel) =>
                {
                    Logger.LogDebug(TODO, $"[KcpClient] 收到数据包");
                    OnDataReceived?.Invoke(data.Array,
                        kcp2kChannel == KcpChannel.Reliable ? EProtocolChannel.Reliable : EProtocolChannel.Unreliable);
                },
                () =>
                {
                    Logger.LogDebug(TODO, $"[KcpClient] 断开连接");
                    OnDisconnected?.Invoke();
                },
                (code, msg) => OnError?.Invoke($"{code}_{msg}"),
                kcp2kConfig);
            _kcp2kConfig = kcp2kConfig;
        }

        public void Connect(string serverIp, ushort serverPort)
        {
            _kcp2kClient.Connect(serverIp, serverPort);
        }

        public void Send(byte[] data, EProtocolChannel channel)
        {
            // 转换为kcp的通道
            var kcp2kChannel = channel == EProtocolChannel.Reliable ? KcpChannel.Reliable : KcpChannel.Unreliable;
            _kcp2kClient.Send(new ArraySegment<byte>(data), kcp2kChannel);
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
