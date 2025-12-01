using Net.FrameSync;
using Net.TCP.Message;
using Net.TCP.Message.C2S;
using Net.TCP.Message.S2C;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

/// <summary>
/// 连接确认消息处理器
/// </summary>
public class S2C_ConnectConfirmMessageHandler : MessageHandler<S2C_ConnectConfirmMessage>
{
    public override S2C_ConnectConfirmMessage TcpMessage {  get; set; }

    public override void HandleMessage(TcpMessage tcpMessage)
    {
        base.HandleMessage(tcpMessage);

        // 设置初始化ID
        NetManager.Instance.InitClientId(TcpMessage.ClientID);
        // 发送绑定消息给服务端（携带本地ID）
        NetManager.Instance.GetTcpClient().EnqueueMessage(new C2S_BindMessage() { ClientID = TcpMessage.ClientID, UdpPort = (NetManager.Instance.udpClientEndPoint as IPEndPoint).Port });
    }
}
