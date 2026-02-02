using System.Net;
using System.Threading.Tasks;
using Core.Mono;
using Core.Net.FrameSync.Udp;
using Core.Net.Tcp;
using Core.Quit;
using Core.Service;
using Core.Singleton;

namespace Core.Net.FrameSync.Manager
{
    /// <summary>
    /// ���������
    /// </summary>
    public class NetManager : SingletonAutoMono<NetManager>, INetManager
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

        /// <summary>
        /// ��������״̬
        /// </summary>
        public bool Connected => _tcpClient != null && _tcpClient.GetTcpConnectState() && _tcpClient.IsConnecting;

        private void Awake()
        {
            ServiceLocator.Get<IQuitHandler>().OnAppQuit += OnAppQuit;
            ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
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
        /// ��ȡUDP
        /// </summary>
        /// <returns></returns>
        public UdpClient GetUdp()
        {
            return _udpClient;
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

        private async Task OnAppQuit()
        {
            RequestCloseConnect();
            await Task.CompletedTask;
        }
    }
}
