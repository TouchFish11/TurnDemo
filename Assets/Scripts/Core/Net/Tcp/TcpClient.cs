using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Core.Global;
using Core.Net.FrameSync.Manager;
using Core.Net.Tcp.Handler;
using Core.Net.Tcp.Message;
using Core.Net.Tcp.Message.C2S;
using Core.Net.Tcp.Message.S2C;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Net.Tcp
{
    /// <summary>
    /// TCP�ͻ���
    /// </summary>
    public class TcpClient
    {
        // tcp�׽���
        private Socket _tcpSocket;
        // �����¼�����
        private SocketAsyncEventArgs _connectEvent;
        // �����¼�����
        private SocketAsyncEventArgs _receiveEvent;
        // �����¼�����
        private SocketAsyncEventArgs _sendEvent;
        // ������Ϣ����
        private readonly Queue<TcpMessage> _sendMassageQueue = new Queue<TcpMessage>();
        // ������Ϣ����
        private readonly Queue<TcpMessage> _receiveMassageQueue = new Queue<TcpMessage>();
        // ������Ϣ����
        private readonly C2S_HeartMessage _c2S_HeartMessage = new C2S_HeartMessage() { ClientID = NetManager.Instance.ClientID };
        // ��Ϣ������
        private readonly byte[] _cacheBuffer = new byte[GlobalSettings.Instance.netModuleConfig.tcpReceiveBufferSize];
        // ��ʱ������
        private readonly byte[] _tempCacheBuffer = new byte[GlobalSettings.Instance.netModuleConfig.tcpReceiveTempBufferSize];
        // ����������
        private int _cacheLength = 0;
        // ����������
        private int nowIndex = 0;
        // ������Ϣ���ͼ����ms��
        private readonly int HeartMsgSendIntervalTime = GlobalSettings.Instance.netModuleConfig.heartMsgSendIntervalTime;
        // �Ƿ����ڷ���
        private bool _isSending;
        // ����������Ϣʱ��
        private long _startHearTimeTicks;
        // ��ϢID����Ϣ������ӳ��
        private readonly Dictionary<Type, IMessageHandler> _idToHandlerMap = new Dictionary<Type, IMessageHandler>();
        /// <summary>
        /// �Ƿ�������
        /// </summary>
        public bool IsConnecting { get; private set; }

        public ConnectData ConnectData { get; private set; } = null;

        /// <summary>
        /// TCP�����ӳٸ���
        /// </summary>
        public event UnityAction<long> OnNetLatencyUpdated;

        public TcpClient()
        {
            // Ĭ��
            _idToHandlerMap.Add(typeof(S2C_HeartMessage), new S2C_HeartMessageHandler());
            _idToHandlerMap.Add(typeof(S2C_ConnectMessage), new S2C_ConnectMessageHandler());
            _idToHandlerMap.Add(typeof(S2C_ConnectConfirmMessage), new S2C_ConnectConfirmMessageHandler());

            // �Զ��������Ϣ
        }

        /// <summary>
        /// �첽����
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="serverPort"></param>
        public void ConnectAsync()
        {
            // �����ظ�����
            if (_tcpSocket != null && _tcpSocket.Connected)
            {
                return;
            }

            // ��ʼ��TCP�׽���
            _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // ��ʼ�������¼�����
            InitSendEventArgs();
            // ��ʼ�������¼�����
            InitReceiveEventArgs();
            // ��ʼ�������¼�����
            InitConnectEventArgs();

            //�첽����
            _tcpSocket.ConnectAsync(_connectEvent);
        }

        /// <summary>
        /// ����TCP�ӳ�
        /// </summary>
        public void CalcTcpRTT()
        {
            long nowHeartTimeTicks = DateTime.Now.Ticks;
            long tcpMsTicks = nowHeartTimeTicks - _startHearTimeTicks;
            long tcpMs = tcpMsTicks / TimeSpan.TicksPerMillisecond;
            _startHearTimeTicks = nowHeartTimeTicks;
            OnNetLatencyUpdated?.Invoke(tcpMs);
        }

        /// <summary>
        /// ������Ϣ����
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
        /// ���Է�����Ϣ
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
            // �������յ�Tcp���͵���Ϣ
            if (_receiveMassageQueue.TryDequeue(out TcpMessage msg))
            {
                if (_idToHandlerMap.TryGetValue(msg.GetType(), out IMessageHandler handler))
                {
                    handler.HandleMessage(msg);
                }
                else
                {
                    Debug.LogError($"δʵ����Ϣ�����߼����޷���������ϢID��{msg}");
                }
            }
        }

        /// <summary>
        /// ��ȡTCP����״̬
        /// </summary>
        /// <returns></returns>
        public bool GetTcpConnectState()
        {
            return _tcpSocket != null && _tcpSocket.Connected;
        }

        /// <summary>
        /// ����ر����ӣ��ͻ��������Ͽ����ӣ�
        /// </summary>
        public void RequestCloseConnect()
        {
            if (_tcpSocket == null)
            {
                return;
            }

            // ͬ�������˳�������Ϣ  TCP�����˳���Ϣ
            EnqueueMessage(new C2S_QuitRequestMessage() { ClientID = NetManager.Instance.ClientID });
        }

        /// <summary>
        /// �ر�����
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
                ConnectData = new ConnectData() { isConnected = IsConnecting };  // ����
                _tcpSocket.Shutdown(SocketShutdown.Both);
                _tcpSocket.Close();
            }
            _tcpSocket = null;
        }


        /// <summary>
        /// ��ʼ����������Ϣ
        /// </summary>
        public void StartSendHeartMsg()
        {
            // �������ӱ�ʶ
            IsConnecting = true;
            ConnectData = new ConnectData() { isConnected = IsConnecting };

            // �����Է���������Ϣ
            ThreadPool.QueueUserWorkItem(SendHeartMessageThread);

            // ����������Ϣ�߳�
            void SendHeartMessageThread(object obj)
            {
                try
                {
                    while (_tcpSocket != null && _tcpSocket.Connected)
                    {
                        EnqueueMessage(_c2S_HeartMessage);
                        //Debug.Log($"����");
                        _startHearTimeTicks = DateTime.Now.Ticks;
                        //���ڷ���
                        Thread.Sleep(HeartMsgSendIntervalTime);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"������Ϣ����ʧ�ܣ�{ex.Message}");
                }
            }
        }

        /// <summary>
        /// �첽������Ϣ
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
        /// ��ʼ�����������¼�����
        /// </summary>
        private void InitConnectEventArgs()
        {
            _connectEvent = new SocketAsyncEventArgs();
            _connectEvent.RemoteEndPoint = NetManager.Instance.serverEndPoint;
            _connectEvent.Completed += ConnectCallBack;
        }

        /// <summary>
        /// ��ʼ�������¼�����
        /// </summary>
        private void InitReceiveEventArgs()
        {
            _receiveEvent = new SocketAsyncEventArgs();
            _receiveEvent.RemoteEndPoint = NetManager.Instance.serverEndPoint;
            _receiveEvent.SetBuffer(_tempCacheBuffer, 0, _tempCacheBuffer.Length);
            _receiveEvent.Completed += ReceiveCallBack;
        }

        /// <summary>
        /// ��ʼ�������¼�����
        /// </summary>
        private void InitSendEventArgs()
        {
            _sendEvent = new SocketAsyncEventArgs();
            _sendEvent.RemoteEndPoint = NetManager.Instance.serverEndPoint;
            _sendEvent.Completed += SendCallBack;
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="args"></param>
        private void HandleMessage(SocketAsyncEventArgs args)
        {
            try
            {
                // ��ת�������������
                Array.Copy(args.Buffer, 0, _cacheBuffer, _cacheLength, args.BytesTransferred);
                _cacheLength += args.BytesTransferred;

                while (true)
                {
                    int msgID = -1;
                    int msgLength = -1;
                    bool hasHeader = false;

                    // ���ж��Ƿ񹻽�����Ϣͷ��8�ֽڣ�
                    if (_cacheLength - nowIndex >= 8)
                    {
                        msgID = BitConverter.ToInt32(_cacheBuffer, nowIndex);
                        nowIndex += 4;
                        msgLength = BitConverter.ToInt32(_cacheBuffer, nowIndex);
                        nowIndex += 4;
                        hasHeader = true;
                    }

                    // ������Ϣ�壨��ͷ+���壩
                    if (hasHeader && _cacheLength - nowIndex >= msgLength)
                    {
                        // ����������Ϣ
                        TcpMessage baseMassage = TcpMessageFactory.CreateMessage(msgID, _cacheBuffer, nowIndex);
                        if (baseMassage != null)
                        {
                            // ���յ�����Ϣ��������������У��������̷߳���
                            _receiveMassageQueue.Enqueue(baseMassage);
                        }

                        // �ƶ�����������������ǰ��Ϣ��
                        // ������Ϣ��ĳ���
                        nowIndex += msgLength;

                        // ���Ͽͻ���ID�ĳ���
                        // ��4����Ϊ�����л��Ŀͻ���ID��������������������ڷ����л�ʱ����
                        // ���Լ����ģ�����ᵼ��nowIndexֵ������args.BytesTransferred
                        nowIndex += 4;

                        // ����������ѿգ��������������� nowIndex һֱ����
                        if (nowIndex == _cacheLength)
                        {
                            nowIndex = 0;
                            _cacheLength = 0;
                            break; // ���������ˣ��˳�ѭ��
                        }
                    }
                    else
                    {
                        // �ְ�����������ͷ�򲻹��壬��������+�˳�ѭ��
                        if (hasHeader)
                        {
                            // ������ͷ�������壬����8�ֽڣ�ͷ�ĳ��ȣ�
                            nowIndex -= 8;
                        }

                        // �˳�ѭ�����ȴ��´��յ������ټ�������
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"{ex}");
                // �رյ�ǰ�ͻ�������
                CloseConnect();
            }
        }

        /// <summary>
        /// ���ӻص�
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
                Debug.Log("���ڳ������ӷ�����...");
                // �첽������Ϣ
                ReceiveAsync();
            }
            else
            {
                Debug.LogError($"����������ʧ�ܣ�{e.SocketError}");
            }
        }

        /// <summary>
        /// ���ͻص�
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
                Debug.LogError($"������Ϣʧ�ܣ�{e.SocketError}");
            }

            _isSending = false;
            TrySendAsync();
        }

        /// <summary>
        /// ���ջص�
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
                // ֻ��������������Ϣ
                if (e.RemoteEndPoint.Equals(NetManager.Instance.serverEndPoint))
                {
                    // ������Ϣ
                    HandleMessage(e);

                    // �ٴν�����Ϣ
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
                Debug.LogError($"������Ϣʧ�ܣ�{e.SocketError}");
                CloseConnect();
            }
        }
    }
}
