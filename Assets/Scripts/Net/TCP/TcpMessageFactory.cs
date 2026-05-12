using Net.FrameSync.Tcp.Message;
using Net.FrameSync.Tcp.Message.S2C;
using UnityEngine;

/// <summary>
/// TCP��Ϣ����
/// </summary>
public class TcpMessageFactory
{
    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <param name="msgId"></param>
    /// <param name="bytes"></param>
    /// <param name="nowIndex"></param>
    /// <returns></returns>
    public static TcpMessage CreateMessage(int msgId, byte[] bytes, int nowIndex)
    {
        TcpMessage tcpMessage = null;
        //������Ϣ��
        switch (msgId)
        {
            case 2000:
                tcpMessage = new S2C_HeartMessage();
                break;
            case 2001:
                tcpMessage = new S2C_ConnectMessage();
                break;
            case 2002:
                tcpMessage = new S2C_MatchSuccessMessage();
                break;
            case 2003:
                tcpMessage = new S2C_ConfirmMessage();
                break;
            case 2004:
                tcpMessage = new S2C_PrepareReceMessage();
                break;
            case 2005:
                tcpMessage = new S2C_StartRaceMessage();
                break;
            case 2006:
                tcpMessage = new S2C_LeaveRaceMessage();
                break;
            case 2007:
                tcpMessage = new S2C_ReconnecRaceMessage();
                break;
            case 2008:
                tcpMessage = new S2C_ConnectConfirmMessage();
                break;
            default:
                Debug.LogError($"δ�������Ϣ���ͣ�{msgId}");
                break;
        }

        // ������Ϣ
        tcpMessage.Deserialize(bytes, nowIndex);
        return tcpMessage;
    }
}
