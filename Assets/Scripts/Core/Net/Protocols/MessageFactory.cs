using System;
using System.Collections.Generic;
using Core.Net.Protocols.Tcp.Messages.Common;
using Net.Protocols.Configs;
using Net.Protocols.FSync.Messages;
using Net.Protocols.Tcp;
using Net.Protocols.Tcp.Messages.Battle.S2C;

namespace Net.Protocols
{
    /// <summary>
    /// 消息工厂
    /// </summary>
    public static class MessageFactory
    {
        private static readonly Dictionary<int, Message> _idToMessage = new();

        static MessageFactory()
        {
            
        }
        
        /// <summary>
        /// 创建消息
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="bytes"></param>
        /// <param name="nowIndex"></param>
        /// <returns></returns>
        public static Message CreateMessage(int msgId, byte[] bytes, int nowIndex)
        {
            Message message = msgId switch
            {
                MessageIDConfig.S2C_Connect_ID => new S2C_ConnectMessage(),
                MessageIDConfig.S2C_Disconnect_ID => new S2C_DisConnectMessage(),
                MessageIDConfig.S2C_MatchSuccess_ID => new S2C_MatchSuccessMessage(),
                MessageIDConfig.S2C_PrepareRace_ID => new S2C_PrepareRaceMessage(),
                MessageIDConfig.S2C_StartRace_ID => new S2C_StartRaceMessage(),
                MessageIDConfig.Heartbeat_ID => new HeartMessage(),
                MessageIDConfig.LeaveRace_ID => new LeaveRaceMessage(),
                MessageIDConfig.ReconnectRace_ID => new ReconnectRaceMessage(),
                MessageIDConfig.S2C_Frame_ID => new S2C_FrameMessage(),
                MessageIDConfig.S2C_Notice_MatchConfirmState_ID => new S2C_ClientConfirmMatchStateMessage(),
                MessageIDConfig.S2C_ClientJoin_ID => new S2C_ClientJoinMessage(),
                _ => throw new Exception($"Unknown message id: {msgId}")
            };

            // 序列化消息体
            message.Deserialize(bytes, nowIndex);
            return message;
        }

        public static int GetMessageID(Message message)
        {
            return message switch
            {
                TcpMessage tcpMessage => tcpMessage.GetMsgID(),
                C2S_NextFrameMessage => C2S_NextFrameMessage.FrameMessageID,
                S2C_FrameMessage => S2C_FrameMessage.FrameMessageID,
                _ => throw new ArgumentOutOfRangeException(message.ToString())
            };
        }
    }
}
