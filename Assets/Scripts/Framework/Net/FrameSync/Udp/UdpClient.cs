using Net.FrameSync.Command;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Net.FrameSync.UDP
{
    /// <summary>
    /// UDP客户端
    /// </summary>
    public class UdpClient
    {
        // Udp套接字
        private Socket _udpSocket;
        // 接收消息事件参数
        private readonly SocketAsyncEventArgs _receiveFromEventArgs;
        // 发送消息事件参数
        private readonly SocketAsyncEventArgs _sendToEventArgs;
        // 接收指令队列
        private readonly Queue<FrameCommand> _receiveFromQueue = new Queue<FrameCommand>();
        // 发送指令队列
        private readonly Queue<FrameCommand> _sendToQueue = new Queue<FrameCommand>();
        // 缓冲区
        private readonly byte[] _cacheBuffer = new byte[GlobalSettings.Instance.UdpReceiveBufferSize];
        // 是否正在连接
        private bool _isConnected;
        // 是否正在发送
        private volatile bool _isSending;
        // 帧同步处理器
        private readonly FSFrameHandler fSFrameHandler;

        /// <summary>
        /// 记录客户端已经执行完成的服务器全局帧 ID
        /// </summary>
        public int LocalFrameID => fSFrameHandler.FrameId;

        public UdpClient()
        {
            _receiveFromEventArgs = new SocketAsyncEventArgs();
            _sendToEventArgs = new SocketAsyncEventArgs();
            fSFrameHandler = new FSFrameHandler();
            _sendToEventArgs.Completed += SendToCallBack;
        }

        /// <summary>
        /// 绑定
        /// </summary>
        public void Bind(ref EndPoint endPoint)
        {
            if (_udpSocket != null && _isConnected)
            {
                return;
            }

            try
            {
                // 创建Udp套接字
                _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // 绑定本机地址
                _udpSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
                _isConnected = true;

                // 异步接收消息
                ReceiveFromAsync();
                // 记录udp动态绑定的端点
                endPoint = _udpSocket.LocalEndPoint;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"UDP绑定失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 异步接收消息
        /// </summary>
        private void ReceiveFromAsync()
        {
            _receiveFromEventArgs.RemoteEndPoint = NetManager.Instance.serverEndPoint;
            _receiveFromEventArgs.SetBuffer(_cacheBuffer, 0, _cacheBuffer.Length);
            _receiveFromEventArgs.Completed += ReceiveCompleted;
            bool isPending = _udpSocket.ReceiveFromAsync(_receiveFromEventArgs);
            if (!isPending)
            {
                ReceiveCompleted(_udpSocket, _receiveFromEventArgs);
            }
        }

        /// <summary>
        /// 接收消息回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (!_isConnected)
            {
                return;
            }

            if (e.SocketError == SocketError.Success)
            {
                // 只处理服务器发送的消息
                if (e.RemoteEndPoint.Equals(NetManager.Instance.serverEndPoint))
                {
                    // 解析消息
                    ParseMessages(e);
                }
            }
            else
            {
                Debug.LogError($"接收消息失败，消息长度：{e.BytesTransferred}，错误：{e.SocketError}");
            }

            if (_udpSocket != null && _isConnected)
            {
                // 从0开始接收
                e.SetBuffer(0, _cacheBuffer.Length);
                // 继续接收消息
                bool isPending = _udpSocket.ReceiveFromAsync(e);
                if (!isPending)
                {
                    ReceiveCompleted(sender, e);
                }
            }
        }

        /// <summary>
        /// 解析消息
        /// </summary>
        private void ParseMessages(SocketAsyncEventArgs e)
        {
            byte[] bytes = new byte[e.BytesTransferred];
            Array.Copy(e.Buffer, 0, bytes, 0, e.BytesTransferred);

            //解析消息
            S2C_FrameCommand s2C_FrameCommand = new S2C_FrameCommand();
            s2C_FrameCommand.Deserialize(bytes);
            _receiveFromQueue.Enqueue(s2C_FrameCommand);
        }

        /// <summary>
        /// 放入指令
        /// </summary>
        /// <param name="message"></param>
        public void EnqueueCommand(FrameCommand frameCommand)
        {
            // 放入指令队列
            _sendToQueue.Enqueue(frameCommand);

            // 尝试发送（无需判断返回值，失败只是队列未及时处理，后续会重试）
            TrySendToAsync();
        }

        /// <summary>
        /// 尝试异步发送指令
        /// </summary>
        /// <returns></returns>
        private void TrySendToAsync()
        {
            if (_udpSocket == null)
            {
                Debug.LogError("UDP Socket未初始化或未连接");
                return;
            }

            // 正在发送则不处理
            if (_isSending)
            {
                Debug.Log($"消息发送失败，原因：消息正在发送，_isSending为{_isSending}");
                return;
            }

            if (_sendToQueue.TryDequeue(out FrameCommand frameCommand))
            {
                _isSending = true;
                byte[] bytes = frameCommand.Serialize();
                _sendToEventArgs.RemoteEndPoint = NetManager.Instance.serverEndPoint;
                _sendToEventArgs.SetBuffer(bytes, 0, bytes.Length);

                bool isPending = _udpSocket.SendToAsync(_sendToEventArgs);
                if (!isPending)
                {
                    // 同步完成，直接处理结果（手动触发回调逻辑）
                    SendToCallBack(_udpSocket, _sendToEventArgs);
                }
                // 异步进行时，回调会处理，无需额外操作
            }
            else
            {
                // 队列空了，重置发送状态
                _isSending = false;
            }
        }

        /// <summary>
        /// 发送消息回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendToCallBack(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                Debug.LogError($"消息发送失败：{e.SocketError}");
            }

            // 标记发送完成
            _isSending = false;
            // 继续发送队列中的下一条消息
            TrySendToAsync();
        }

        /// <summary>
        /// 帧更新
        /// </summary>
        public void OnUpdate()
        {
            // 处理接收到Udp发送的消息
            if (_receiveFromQueue.TryDequeue(out FrameCommand command))
            {
                // 执行
                fSFrameHandler.ServerCommandInput(command as S2C_FrameCommand);
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            if (_udpSocket == null)
            {
                return;
            }

            _isConnected = false;
            _udpSocket.Close();
            _udpSocket = null;
        }
    }
}
