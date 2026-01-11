using Framework;
using Net.FrameSync;
using Net.TCP.Message;
using Net.TCP.Message.S2C;
using UnityEngine;

/// <summary>
/// 服务器发送客户端_连接消息处理器
/// </summary>
public class S2C_ConnectMessageHandler : MessageHandler<S2C_ConnectMessage>
{
    public override S2C_ConnectMessage TcpMessage { get; set; }

    public override void HandleMessage(TcpMessage tcpMessage)
    {
        base.HandleMessage(tcpMessage);

        // 连接
        if (TcpMessage.ConnectState)
        {
            // 开启心跳消息的发送
            NetManager.Instance.GetTcpClient().StartSendHeartMsg();
            Debug.Log($"连接服务器成功");
        }
        // 断开
        else
        {
            NetManager.Instance.CloseConnect(TcpMessage.ClientID);
        }

        // 连接完成后
        EventCenter.Instance.TriggerEvent(new PostConnectedEvent() { S2C_ConnectMessage = TcpMessage });
    }
}
