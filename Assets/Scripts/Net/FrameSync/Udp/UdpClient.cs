using Net.FrameSync.Command;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Core.Global;
using Net.FrameSync.Handler;
using Net.FrameSync.Manager;
using UnityEngine;

namespace Net.FrameSync.UDP
{
    /// <summary>
    /// UDP�ͻ���
    /// </summary>
    public class UdpClient
    {
        // Udp�׽���
        private Socket _udpSocket;
        // ������Ϣ�¼�����
        private readonly SocketAsyncEventArgs _receiveFromEventArgs;
        // ������Ϣ�¼�����
        private readonly SocketAsyncEventArgs _sendToEventArgs;
        // ����ָ�����
        private readonly Queue<FrameCommand> _receiveFromQueue = new Queue<FrameCommand>();
        // ����ָ�����
        private readonly Queue<FrameCommand> _sendToQueue = new Queue<FrameCommand>();
        // ������
        private readonly byte[] _cacheBuffer = new byte[GlobalSettings.Instance.udpReceiveBufferSize];
        // �Ƿ���������
        private bool _isConnected;
        // �Ƿ����ڷ���
        private volatile bool _isSending;
        // ֡ͬ��������
        private readonly FSFrameHandler fSFrameHandler;

        /// <summary>
        /// ��¼�ͻ����Ѿ�ִ����ɵķ�����ȫ��֡ ID
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
        /// ��
        /// </summary>
        public void Bind(ref EndPoint endPoint)
        {
            if (_udpSocket != null && _isConnected)
            {
                return;
            }

            try
            {
                // ����Udp�׽���
                _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // �󶨱�����ַ
                _udpSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
                _isConnected = true;

                // �첽������Ϣ
                ReceiveFromAsync();
                // ��¼udp��̬�󶨵Ķ˵�
                endPoint = _udpSocket.LocalEndPoint;
            }
            catch (Exception ex)
            {
                Debug.LogError($"UDP��ʧ�ܣ�{ex.Message}");
            }
        }

        /// <summary>
        /// �첽������Ϣ
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
        /// ������Ϣ�ص�
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
                // ֻ�������������͵���Ϣ
                if (e.RemoteEndPoint.Equals(NetManager.Instance.serverEndPoint))
                {
                    // ������Ϣ
                    ParseMessages(e);
                }
            }
            else
            {
                Debug.LogError($"������Ϣʧ�ܣ���Ϣ���ȣ�{e.BytesTransferred}������{e.SocketError}");
            }

            if (_udpSocket != null && _isConnected)
            {
                // ��0��ʼ����
                e.SetBuffer(0, _cacheBuffer.Length);
                // ����������Ϣ
                bool isPending = _udpSocket.ReceiveFromAsync(e);
                if (!isPending)
                {
                    ReceiveCompleted(sender, e);
                }
            }
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        private void ParseMessages(SocketAsyncEventArgs e)
        {
            byte[] bytes = new byte[e.BytesTransferred];
            Array.Copy(e.Buffer, 0, bytes, 0, e.BytesTransferred);

            //������Ϣ
            S2C_FrameCommand s2C_FrameCommand = new S2C_FrameCommand();
            s2C_FrameCommand.Deserialize(bytes);
            _receiveFromQueue.Enqueue(s2C_FrameCommand);
        }

        /// <summary>
        /// ����ָ��
        /// </summary>
        /// <param name="frameCommand"></param>
        public void EnqueueCommand(FrameCommand frameCommand)
        {
            // ����ָ�����
            _sendToQueue.Enqueue(frameCommand);

            // ���Է��ͣ������жϷ���ֵ��ʧ��ֻ�Ƕ���δ��ʱ���������������ԣ�
            TrySendToAsync();
        }

        /// <summary>
        /// �����첽����ָ��
        /// </summary>
        /// <returns></returns>
        private void TrySendToAsync()
        {
            if (_udpSocket == null)
            {
                Debug.LogError("UDP Socketδ��ʼ����δ����");
                return;
            }

            // ���ڷ����򲻴���
            if (_isSending)
            {
                Debug.Log($"��Ϣ����ʧ�ܣ�ԭ����Ϣ���ڷ��ͣ�_isSendingΪ{_isSending}");
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
                    // ͬ����ɣ�ֱ�Ӵ���������ֶ������ص��߼���
                    SendToCallBack(_udpSocket, _sendToEventArgs);
                }
                // �첽����ʱ���ص��ᴦ��������������
            }
            else
            {
                // ���п��ˣ����÷���״̬
                _isSending = false;
            }
        }

        /// <summary>
        /// ������Ϣ�ص�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendToCallBack(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                Debug.LogError($"��Ϣ����ʧ�ܣ�{e.SocketError}");
            }

            // ��Ƿ������
            _isSending = false;
            // �������Ͷ����е���һ����Ϣ
            TrySendToAsync();
        }

        /// <summary>
        /// ֡����
        /// </summary>
        public void OnUpdate()
        {
            // �������յ�Udp���͵���Ϣ
            if (_receiveFromQueue.TryDequeue(out FrameCommand command))
            {
                // ִ��
                fSFrameHandler.ServerCommandInput(command as S2C_FrameCommand);
            }
        }

        /// <summary>
        /// �ر�
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
