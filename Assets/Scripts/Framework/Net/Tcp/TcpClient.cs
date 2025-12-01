using Framework;
using Net.FrameSync;
using Net.TCP.Message;
using Net.TCP.Message.C2S;
using Net.TCP.Message.S2C;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace Net.TCP
{
    public class ConnectData
    {
        // 是否连接
        public bool isConnected;
    }

    /// <summary>
    /// TCP客户端
    /// </summary>
    public class TcpClient
    {
        // tcp套接字
        private Socket _tcpSocket;
        // 连接事件参数
        private SocketAsyncEventArgs _connectEvent;
        // 接收事件参数
        private SocketAsyncEventArgs _receiveEvent;
        // 发送事件参数
        private SocketAsyncEventArgs _sendEvent;
        // 发送消息队列
        private readonly Queue<TcpMessage> _sendMassageQueue = new Queue<TcpMessage>();
        // 接收消息队列
        private readonly Queue<TcpMessage> _receiveMassageQueue = new Queue<TcpMessage>();
        // 心跳消息缓存
        private readonly C2S_HeartMessage _c2S_HeartMessage = new C2S_HeartMessage() { ClientID = NetManager.Instance.ClientID };
        // 消息缓冲区
        private readonly byte[] _cacheBuffer = new byte[GlobalSettings.Instance.TcpReceiveBufferSize];
        // 临时缓冲区
        private readonly byte[] _tempCacheBuffer = new byte[GlobalSettings.Instance.TcpReceiveTempBufferSize];
        // 缓冲区长度
        private int _cacheLength = 0;
        // 缓冲区索引
        private int nowIndex = 0;
        // 心跳消息发送间隔（ms）
        private readonly int HeartMsgSendIntervalTime = GlobalSettings.Instance.HeartMsgSendIntervalTime;
        // 是否正在发送
        private bool _isSending;
        // 发送心跳消息时间
        private long _startHearTimeTicks;
        // 消息ID到消息处理器映射
        private readonly Dictionary<Type, IMessageHandler> _idToHandlerMap = new Dictionary<Type, IMessageHandler>();
        /// <summary>
        /// 是否连接中
        /// </summary>
        public bool IsConnecting { get; private set; }

        public ConnectData ConnectData { get; private set; } = null;

        /// <summary>
        /// TCP网络延迟更新
        /// </summary>
        public event UnityAction<long> OnNetLatencyUpdated;

        public TcpClient()
        {
            // 默认
            _idToHandlerMap.Add(typeof(S2C_HeartMessage), new S2C_HeartMessageHandler());
            _idToHandlerMap.Add(typeof(S2C_ConnectMessage), new S2C_ConnectMessageHandler());
            _idToHandlerMap.Add(typeof(S2C_ConnectConfirmMessage), new S2C_ConnectConfirmMessageHandler());

            // 自定义比赛消息
        }

        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="serverPort"></param>
        public void ConnectAsync()
        {
            // 避免重复连接
            if (_tcpSocket != null && _tcpSocket.Connected)
            {
                return;
            }

            // 初始化TCP套接字
            _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // 初始化发送事件参数
            InitSendEventArgs();
            // 初始化接收事件参数
            InitReceiveEventArgs();
            // 初始化连接事件参数
            InitConnectEventArgs();

            //异步连接
            _tcpSocket.ConnectAsync(_connectEvent);
        }

        /// <summary>
        /// 计算TCP延迟
        /// </summary>
        public void CalcTcpRTT()
        {
            long nowHeartTimeTicks = System.DateTime.Now.Ticks;
            long tcpMsTicks = nowHeartTimeTicks - _startHearTimeTicks;
            long tcpMs = tcpMsTicks / TimeSpan.TicksPerMillisecond;
            _startHearTimeTicks = nowHeartTimeTicks;
            OnNetLatencyUpdated?.Invoke(tcpMs);
        }

        /// <summary>
        /// 放入消息队列
        /// </summary>
        /// <param name="message"></param>
        public void EnqueueMessage(TcpMessage message)
        {
            lock (_sendMassageQueue)
            {
                _sendMassageQueue.Enqueue(message);

                TrySendAsync();
            }
        }

        /// <summary>
        /// 尝试发送消息
        /// </summary>
        /// <returns></returns>
        private bool TrySendAsync()
        {
            if (_isSending)
            {
                return false;
            }

            lock (_sendMassageQueue)
            {
                if (_sendMassageQueue.TryDequeue(out TcpMessage message))
                {
                    byte[] bytes = message.Serialize();
                    _sendEvent.SetBuffer(bytes, 0, bytes.Length);
                    bool isPending = _tcpSocket.SendAsync(_sendEvent);
                    if (!isPending)
                    {
                        SendCallBack(_tcpSocket, _sendEvent);
                    }

                    _isSending = true;
                    return true;
                }
            }

            return false;
        }

        public void OnUpdate()
        {
            // 处理接收到Tcp发送的消息
            if (_receiveMassageQueue.TryDequeue(out TcpMessage msg))
            {
                if (_idToHandlerMap.TryGetValue(msg.GetType(), out IMessageHandler handler))
                {
                    handler.HandleMessage(msg);
                }
                else
                {
                    Debug.LogError($"未实现消息处理逻辑，无法处理：消息ID：{msg}");
                }
            }
        }

        /// <summary>
        /// 获取TCP连接状态
        /// </summary>
        /// <returns></returns>
        public bool GetTcpConnectState()
        {
            return _tcpSocket != null && _tcpSocket.Connected;
        }

        /// <summary>
        /// 请求关闭连接（客户端主动断开连接）
        /// </summary>
        public void RequestCloseConnect()
        {
            if (_tcpSocket == null)
            {
                return;
            }

            // 同步发送退出请求消息  TCP处理退出消息
            EnqueueMessage(new C2S_QuitRequestMessage() { ClientID = NetManager.Instance.ClientID });
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConnect()
        {
            if (!IsConnecting)
            {
                return;
            }

            if (_tcpSocket == null)
            {
                return;
            }

            if (_tcpSocket.Connected)
            {
                IsConnecting = false;
                ConnectData = new ConnectData() { isConnected = IsConnecting };  // 测试
                _tcpSocket.Shutdown(SocketShutdown.Both);
                _tcpSocket.Close();
            }
            _tcpSocket = null;
        }


        /// <summary>
        /// 开始发送心跳消息
        /// </summary>
        public void StartSendHeartMsg()
        {
            // 更新连接标识
            IsConnecting = true;
            ConnectData = new ConnectData() { isConnected = IsConnecting };

            // 周期性发送心跳消息
            ThreadPool.QueueUserWorkItem(SendHeartMessageThread);

            // 发送心跳消息线程
            void SendHeartMessageThread(object obj)
            {
                try
                {
                    while (_tcpSocket != null && _tcpSocket.Connected)
                    {
                        EnqueueMessage(_c2S_HeartMessage);
                        //Debug.Log($"发送");
                        _startHearTimeTicks = System.DateTime.Now.Ticks;
                        //周期发送
                        Thread.Sleep(HeartMsgSendIntervalTime);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"心跳消息发送失败：{ex.Message}");
                }
            }
        }

        /// <summary>
        /// 异步接收消息
        /// </summary>
        private void ReceiveAsync()
        {
            bool isPending = _tcpSocket.ReceiveAsync(_receiveEvent);
            if (!isPending)
            {
                ReceiveCallBack(_tcpSocket, _receiveEvent);
            }
        }

        /// <summary>
        /// 初始化网络连接事件参数
        /// </summary>
        private void InitConnectEventArgs()
        {
            _connectEvent = new SocketAsyncEventArgs();
            _connectEvent.RemoteEndPoint = NetManager.Instance.serverEndPoint;
            _connectEvent.Completed += ConnectCallBack;
        }

        /// <summary>
        /// 初始化接收事件参数
        /// </summary>
        private void InitReceiveEventArgs()
        {
            _receiveEvent = new SocketAsyncEventArgs();
            _receiveEvent.RemoteEndPoint = NetManager.Instance.serverEndPoint;
            _receiveEvent.SetBuffer(_tempCacheBuffer, 0, _tempCacheBuffer.Length);
            _receiveEvent.Completed += ReceiveCallBack;
        }

        /// <summary>
        /// 初始化发送事件参数
        /// </summary>
        private void InitSendEventArgs()
        {
            _sendEvent = new SocketAsyncEventArgs();
            _sendEvent.RemoteEndPoint = NetManager.Instance.serverEndPoint;
            _sendEvent.Completed += SendCallBack;
        }

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="args"></param>
        private void HandleMessage(SocketAsyncEventArgs args)
        {
            try
            {
                // 先转存进缓存数组中
                Array.Copy(args.Buffer, 0, _cacheBuffer, _cacheLength, args.BytesTransferred);
                _cacheLength += args.BytesTransferred;

                while (true)
                {
                    int msgID = -1;
                    int msgLength = -1;
                    bool hasHeader = false;

                    // 先判断是否够解析消息头（8字节）
                    if (_cacheLength - nowIndex >= 8)
                    {
                        msgID = BitConverter.ToInt32(_cacheBuffer, nowIndex);
                        nowIndex += 4;
                        msgLength = BitConverter.ToInt32(_cacheBuffer, nowIndex);
                        nowIndex += 4;
                        hasHeader = true;
                    }

                    // 解析消息体（够头+够体）
                    if (hasHeader && _cacheLength - nowIndex >= msgLength)
                    {
                        // 处理具体消息
                        TcpMessage baseMassage = TcpMessageFactory.CreateMessage(msgID, _cacheBuffer, nowIndex);
                        if (baseMassage != null)
                        {
                            // 将收到的消息解析后放入容器中，方便主线程访问
                            _receiveMassageQueue.Enqueue(baseMassage);
                        }

                        // 移动解析索引，跳过当前消息体
                        // 加上消息体的长度
                        nowIndex += msgLength;

                        // 加上客户端ID的长度
                        // 加4是因为，序列化的客户端ID不会在这里解析，而是在反序列化时解析
                        // 所以加上四，否则会导致nowIndex值不等于args.BytesTransferred
                        nowIndex += 4;

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
                Debug.LogError($"{ex}");
                // 关闭当前客户端连接
                CloseConnect();
            }
        }

        /// <summary>
        /// 连接回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectCallBack(object sender, SocketAsyncEventArgs e)
        {
            if (_tcpSocket == null || !_tcpSocket.Connected)
            {
                return;
            }

            if (e.SocketError == SocketError.Success)
            {
                Debug.Log("正在尝试连接服务器...");
                // 异步接收消息
                ReceiveAsync();
            }
            else
            {
                Debug.LogError($"服务器连接失败：{e.SocketError}");
            }
        }

        /// <summary>
        /// 发送回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendCallBack(object sender, SocketAsyncEventArgs e)
        {
            if (_tcpSocket == null || !_tcpSocket.Connected)
            {
                return;
            }

            if (e.SocketError != SocketError.Success)
            {
                Debug.LogError($"发送消息失败：{e.SocketError}");
            }

            _isSending = false;
            TrySendAsync();
        }

        /// <summary>
        /// 接收回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiveCallBack(object sender, SocketAsyncEventArgs e)
        {
            if (_tcpSocket == null || !_tcpSocket.Connected)
            {
                return;
            }

            if (e.SocketError == SocketError.Success)
            {
                // 只处理服务器的消息
                if (e.RemoteEndPoint.Equals(NetManager.Instance.serverEndPoint))
                {
                    // 处理消息
                    HandleMessage(e);

                    // 再次接收消息
                    if (_tcpSocket == null || !_tcpSocket.Connected)
                    {
                        return;
                    }

                    e.SetBuffer(0, _tempCacheBuffer.Length);
                    bool isPending = e.ConnectSocket.ReceiveAsync(e);
                    if (!isPending)
                    {
                        ReceiveCallBack(sender, e);
                    }
                }
            }
            else
            {
                Debug.LogError($"接收消息失败：{e.SocketError}");
                CloseConnect();
            }
        }
    }
}
