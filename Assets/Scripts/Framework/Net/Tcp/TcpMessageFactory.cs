using Net.TCP.Message;
using Net.TCP.Message.S2C;
using UnityEngine;

/// <summary>
/// TCP消息工厂
/// </summary>
public class TcpMessageFactory
{
    /// <summary>
    /// 创建消息
    /// </summary>
    /// <param name="msgId"></param>
    /// <param name="bytes"></param>
    /// <param name="nowIndex"></param>
    /// <returns></returns>
    public static TcpMessage CreateMessage(int msgId, byte[] bytes, int nowIndex)
    {
        TcpMessage tcpMessage = null;
        //解析消息体
        switch (msgId)
        {
            case 2000:
                tcpMessage = new S2C_HeartMessage();
                break;
            case 2001:
                tcpMessage = new S2C_ConnectMessage();
                break;
            case 2008:
                tcpMessage = new S2C_ConnectConfirmMessage();
                break;
            default:
                Debug.LogError($"未定义的消息类型：{msgId}");
                break;
        }

        // 解析消息
        tcpMessage.Deserialize(bytes, nowIndex);
        return tcpMessage;
    }
}
