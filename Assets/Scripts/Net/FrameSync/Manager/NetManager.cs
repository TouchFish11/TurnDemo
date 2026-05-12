using System.Net;
using Core.Singleton;
using Net.FrameSync.Command;
using Net.FrameSync.Tcp;
using Net.FrameSync.Tcp.Message;
using Net.FrameSync.UDP;

namespace Net.FrameSync.Manager
{
    /// <summary>
    /// ���������
    /// </summary>
    public class NetManager : SingletonAutoMono<NetManager>
    {
        // Tcp�߼�
        private TcpClient _tcpClient;
        // Udp�߼�
        private UdpClient _udpClient;
        // �������˵�
        public EndPoint serverEndPoint;
        // ����UDP�˵�
        public EndPoint udpClientEndPoint;

        /// <summary>
        /// �ͻ���ID
        /// </summary>
        public int ClientID { get; private set; }

        public bool Connected => _tcpClient != null && _tcpClient.GetTcpConnectState(); 

        private void Awake()
        {
            //MonoAdapter.Instance.AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// ��ȡTCP�ͻ��˶���
        /// </summary>
        /// <returns></returns>
        public TcpClient GetTcpClient()
        {
            return _tcpClient;
        }

        /// <summary>
        /// �����ͻ���
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="serverPort"></param>
        public void StartClient(string serverIp, int serverPort)
        {
            // ָ���������˵�
            serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

            // ��ʼ��TCP��UDP�߼�
            _tcpClient = new TcpClient();
            _udpClient = new UdpClient();

            // Tcp�첽����
            _tcpClient.ConnectAsync();
            // Udp��
            _udpClient.Bind(ref udpClientEndPoint);
        }

        /// <summary>
        /// ��ʼ���ͻ���ID
        /// </summary>
        /// <param name="clientId"></param>
        public void InitClientId(int clientId)
        {
            ClientID = clientId;
        }

        /// <summary>
        /// TCP�첽������Ϣ
        /// </summary>
        /// <param name="baseMessage"></param>
        public void SendAsync(TcpMessage baseMessage)
        {
            _tcpClient.EnqueueMessage(baseMessage);
        }

        /// <summary>
        /// UDP�첽����ָ��
        /// </summary>
        /// <param name="frameCommand"></param>
        public void SendToAsync(FrameCommand frameCommand)
        {
            _udpClient.EnqueueCommand(frameCommand);
        }

        /// <summary>
        /// ����ر�����
        /// </summary>
        public void RequestCloseConnect()
        {
            if (_tcpClient == null || _udpClient == null)
            {
                return;
            }

            _tcpClient.RequestCloseConnect();
            _udpClient.Close();
        }

        /// <summary>
        /// �ر�����
        /// </summary>
        public void CloseConnect(int clientID)
        {
            if (_tcpClient == null || _udpClient == null)
            {
                return;
            }

            // ������Ϣ�ᷢ�͸����еĿͻ��ˣ����ԶϿ����ӵ�ID���������ͻ���ID�Ͳ��ô���
            if (clientID != ClientID)
            {
                return;
            }

            _tcpClient.CloseConnect();
            _udpClient.Close();

            _tcpClient = null;
            _udpClient = null;
        }

        /// <summary>
        /// ֡����
        /// </summary>
        private void OnUpdate()
        {
            _tcpClient?.OnUpdate();
            _udpClient?.OnUpdate();
        }

        private void OnDisable()
        {
            RequestCloseConnect();
            //MonoAdapter.Instance.RemoveUpdateListener(OnUpdate);
        }
    }
}
