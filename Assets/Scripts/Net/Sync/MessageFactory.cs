using System;
using Core.Log;
using Net.Sync.Msg.S2C;

namespace Net.Sync
{
    /// <summary>
    /// 消息工厂
    /// </summary>
    public static class MessageFactory
    {
        /// <summary>
        /// 创建消息
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="bytes"></param>
        /// <param name="nowIndex"></param>
        /// <returns></returns>
        public static Message CreateMessage(int msgId, byte[] bytes, int nowIndex)
        {
            Message message = null;
            switch (msgId)
            {
                // case 2000:
                //     message = new S2C_HeartMessage();
                //     break;
                case 2001:
                    message = new ConnectMessage();
                    Logger.Log($"[MessageFactory] 创建连接消息");
                    break;
                // case 2002:
                //     message = new S2C_MatchSuccessMessage();
                //     break;
                // case 2003:
                //     message = new S2C_ConfirmMessage();
                //     break;
                // case 2004:
                //     message = new S2C_PrepareReceMessage();
                //     break;
                // case 2005:
                //     message = new S2C_StartRaceMessage();
                //     break;
                // case 2006:
                //     message = new S2C_LeaveRaceMessage();
                //     break;
                // case 2007:
                //     message = new S2C_ReconnecRaceMessage();
                //     break;
                // case 2008:
                //     message = new S2C_ConnectConfirmMessage();
                //     break;
                default:
                    Logger.LogError($"无效的消息ID：{msgId}");
                    return null;
            }

            // 序列化消息体
            message.Deserialize(bytes, nowIndex);
            return message;
        }

        public static int GetMessageID(Message message)
        {
            if (message is ConnectMessage)
            {
                return 2001;
            }
            
            throw new ArgumentOutOfRangeException(message.ToString());
        }
    }
}
