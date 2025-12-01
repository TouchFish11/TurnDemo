using Framework;
using Net.FrameSync;
using Net.TCP.Message;
using Net.TCP.Message.S2C;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 心跳消息处理器
/// </summary>
public class S2C_HeartMessageHandler : MessageHandler<S2C_HeartMessage>
{
    public override S2C_HeartMessage TcpMessage { get; set; }


    public override void HandleMessage(TcpMessage tcpMessage)
    {
        base.HandleMessage(tcpMessage);

        NetManager.Instance.GetTcpClient().CalcTcpRTT();
    }
}
