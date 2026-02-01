using Core.Net.FrameSync.Udp;
using Core.Net.Tcp;

namespace Core.Net.FrameSync.Manager
{
    /// <summary>
    /// ����������ӿ�
    /// </summary>
    public interface INetManager
    {
        int ClientID { get; }
        bool Connected { get; }

        void CloseConnect(int clientID);
        TcpClient GetTcpClient();
        UdpClient GetUdp();
        void InitClientId(int clientId);
        void RequestCloseConnect();
        void StartClient(string serverIp, int serverPort);
    }
}
