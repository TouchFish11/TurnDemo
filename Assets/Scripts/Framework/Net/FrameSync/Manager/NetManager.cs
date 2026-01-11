using Framework;
using Net.FrameSync.UDP;
using Net.TCP;
using Net.TCP.Message;
using System.Net;
using System.Threading.Tasks;

namespace Net.FrameSync
{
    /// <summary>
    /// 网络管理器
    /// </summary>
    public class NetManager : SingletonAutoMono<NetManager>, INetManager
    {
        // Tcp逻辑
        private TcpClient _tcpClient;
        // Udp逻辑
        private UdpClient _udpClient;
        // 服务器端点
        public EndPoint serverEndPoint;
        // 本机UDP端点
        public EndPoint udpClientEndPoint;

        /// <summary>
        /// 客户端ID
        /// </summary>
        public int ClientID { get; private set; }

        /// <summary>
        /// 网络连接状态
        /// </summary>
        public bool Connected => _tcpClient != null && _tcpClient.GetTcpConnectState() && _tcpClient.IsConnecting;

        private void Awake()
        {
            ServiceLocator.Get<IQuitHandler>().OnAppQuit += OnAppQuit;
            MonoManager.Instance.AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// 获取TCP客户端对象
        /// </summary>
        /// <returns></returns>
        public TcpClient GetTcpClient()
        {
            return _tcpClient;
        }

        /// <summary>
        /// 开启客户端
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="serverPort"></param>
        public void StartClient(string serverIp, int serverPort)
        {
            // 指定服务器端点
            serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

            // 初始化TCP和UDP逻辑
            _tcpClient = new TcpClient();
            _udpClient = new UdpClient();

            // Tcp异步连接
            _tcpClient.ConnectAsync();
            // Udp绑定
            _udpClient.Bind(ref udpClientEndPoint);
        }

        /// <summary>
        /// 初始化客户端ID
        /// </summary>
        /// <param name="clientId"></param>
        public void InitClientId(int clientId)
        {
            ClientID = clientId;
        }

        /// <summary>
        /// 获取UDP
        /// </summary>
        /// <returns></returns>
        public UdpClient GetUdp()
        {
            return _udpClient;
        }

        /// <summary>
        /// 请求关闭连接
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
        /// 关闭连接
        /// </summary>
        public void CloseConnect(int clientID)
        {
            if (_tcpClient == null || _udpClient == null)
            {
                return;
            }

            // 连接消息会发送给所有的客户端，所以断开连接的ID不是自身客户端ID就不用处理
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
        /// 帧更新
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
