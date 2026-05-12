using System;
using System.Net.Sockets;

namespace Net.Sync
{
    /// <summary>
    /// 基于TCP/UDP封装的双通道客户端
    /// </summary>
    public class DualChannelClient : IProtocolClient
    {
        private Socket _tcpSocket;
        private Socket _updSocket;
        
        public event Action<byte[], EProtocolChannel> OnDataReceived;
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<string> OnError;

        public void Connect(string serverIp, ushort serverPort)
        {

        }

        public void Send(byte[] data, EProtocolChannel channel)
        {

        }

        public void Tick()
        {
            
        }

        public void Disconnect()
        {
            
        }
    }
}
