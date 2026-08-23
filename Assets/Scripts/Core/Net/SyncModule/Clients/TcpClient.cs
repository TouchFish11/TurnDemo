using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Core.Log;
using Net.Protocols;
using Logger = Core.Log.Logger;

namespace Core.Net.SyncModule.Clients
{
    /// <summary>
    /// 帧同步Tcp客户端
    /// </summary>
    public class TcpClient : IProtocolClient
    {
        private Socket _tcpSocket;
        // 解析数据缓冲区
        private readonly byte[] _dataBuffer;
        // 临时数据缓冲区
        private readonly byte[] _tempBuffer;
        // 当前缓冲区长度
        private int _cacheLength;
        // 当前缓冲区索引
        private int nowIndex;

        public bool IsConnected => _tcpSocket != null && _tcpSocket.Connected;
        
        public event Action<byte[], EProtocolChannel> OnDataReceived;
        
        public event Action OnConnected;
        
        public event Action OnDisconnected;
        
        public event Action<string> OnError;

        public TcpClient(short bufferSize = 4096, short tempBufferSize = 512)
        {
            _dataBuffer = new byte[bufferSize];
            _tempBuffer = new byte[tempBufferSize];
        }
        
        public async void Connect(string serverIp, ushort serverPort)
        {
            try
            {
                if (_tcpSocket != null && _tcpSocket.Connected)
                    return;
                
                _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await _tcpSocket.ConnectAsync(IPAddress.Parse(serverIp), serverPort);
                OnConnected?.Invoke();
                ReceiveAsync();
            }
            catch (Exception e)
            {
                Logger.LogException(ELogTags.Network, e);
                OnError?.Invoke(e.Message);
            }
        }

        public async void SendAsync(byte[] data, EProtocolChannel channel)
        {
            try
            {
                await _tcpSocket.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
            }
            catch (Exception e)
            {
                Logger.LogException(ELogTags.Network, e);
                OnError?.Invoke(e.Message);
            }
        }

        /// <summary>
        /// 异步接收
        /// </summary>
        private async void ReceiveAsync()
        {
            try
            {
                while (_tcpSocket != null && _tcpSocket.Connected)
                {
                    var receive = await _tcpSocket.ReceiveAsync(new ArraySegment<byte>(_tempBuffer), SocketFlags.None);
                    HandleData(receive);
                    await Task.Yield();
                }
            }
            catch (Exception e)
            {
                Logger.LogException(ELogTags.Network, e);
                OnError?.Invoke(e.Message);
            }
        }

        private void HandleData(int receiveNum)
        {
            try
            {
                //先转存进缓存数组中
                Array.Copy(_tempBuffer, 0, _dataBuffer, _cacheLength, receiveNum);
                _cacheLength += receiveNum;

                while (true)
                {
                    var msgBodyLength = -1;
                    var hasHeader = false;

                    // 先判断是否够解析消息头（8字节）
                    if (_cacheLength - nowIndex >= 8)
                    {
                        var msgID = BitConverter.ToInt32(_dataBuffer, nowIndex);
                        nowIndex += 4;
                        msgBodyLength = BitConverter.ToInt32(_dataBuffer, nowIndex);
                        nowIndex += 4;
                        hasHeader = true;
                        //Logger.LogDebug(ELogTags.Network, $"[TCP-Debug]：消息ID：{msgID},长度：{msgLength}");
                    }

                    // 解析消息体（够头+够体）
                    if (hasHeader && _cacheLength - nowIndex >= msgBodyLength)
                    {
                        var msgRawData = new byte[msgBodyLength + 8];
                        Array.Copy(_dataBuffer, nowIndex - 8, msgRawData, 0, msgBodyLength + 8);
                        OnDataReceived?.Invoke(msgRawData, EProtocolChannel.Resolve);
                        
                        // 移动解析索引，跳过当前消息体
                        // 加上消息体的长度(包含客户端ID)
                        nowIndex += msgBodyLength;

                        // 加上客户端ID的长度
                        // 加4是因为，序列化的客户端ID不会在这里解析，而是在反序列化时解析
                        // 所以加上四，否则会导致nowIndex值不等于args.BytesTransferred
                        //nowIndex += 4;

                        // 如果缓冲区已空，重置索引（避免 nowIndex 一直增大）
                        if (nowIndex == _cacheLength)
                        {
                            nowIndex = 0;
                            _cacheLength = 0;
                            break; // 缓冲区空了，退出循环
                        }
                    }
                    else
                    {
                        // 分包场景：不够头或不够体，回退索引+退出循环
                        if (hasHeader)
                        {
                            // 解析了头但不够体，回退8字节（头的长度）
                            nowIndex -= 8;
                        }

                        // 退出循环，等待下次收到数据再继续解析
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ELogTags.Network, ex);
            }
        }

        public void Tick()
        {
            
        }

        public void Disconnect()
        {
            if (_tcpSocket == null)
                return;

            if (_tcpSocket.Connected)
            {
                _tcpSocket.Shutdown(SocketShutdown.Send);
                _tcpSocket.Disconnect(false);
                _tcpSocket.Close();
            }
            _tcpSocket = null;
            OnDisconnected?.Invoke();
        }
    }
}
