using Net.FrameSync.UDP;
using Net.TCP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 网络管理器接口
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
