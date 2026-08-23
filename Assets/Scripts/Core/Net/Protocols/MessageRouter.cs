using System;
using System.Collections.Generic;
using Core.DI;
using Core.Exceptions;
using Core.Net.Protocols.FSync.Handlers;
using Core.Net.Protocols.Tcp.Handlers.S2C;
using Core.Net.Protocols.Tcp.Messages.Common;
using Net.Protocols.FSync.Messages;
using Net.Protocols.Tcp.Handlers.C2S;
using Net.Protocols.Tcp.Messages.Battle.S2C;

namespace Net.Protocols
{
    /// <summary>
    /// 消息路由器
    /// </summary>
    public class MessageRouter
    {
        private readonly Dictionary<Type, IMessageHandler> _handlers = new();

        public MessageRouter()
        {
            RegisterHandler(typeof(S2C_ConnectMessage), DIContainer.Create<S2C_ConnectMessageHandler>());
            RegisterHandler(typeof(S2C_DisConnectMessage), DIContainer.Create<S2C_DisConnectMessageHandler>());
            RegisterHandler(typeof(HeartMessage), DIContainer.Create<S2C_HeartMessageHandler>());
            RegisterHandler(typeof(LeaveRaceMessage), DIContainer.Create<S2C_LeaveRaceMessageHandler>());
            RegisterHandler(typeof(S2C_MatchSuccessMessage), DIContainer.Create<S2C_MatchSuccessMessageHandler>());
            RegisterHandler(typeof(S2C_PrepareRaceMessage), DIContainer.Create<S2C_PrepareRaceMessageHandler>());
            RegisterHandler(typeof(ReconnectRaceMessage), DIContainer.Create<S2C_ReconnectRaceMessageHandler>());
            RegisterHandler(typeof(S2C_StartRaceMessage), DIContainer.Create<S2C_StartRaceMessageHandler>());
            RegisterHandler(typeof(S2C_FrameMessage), DIContainer.Create<S2C_FrameMessageHandler>());
            RegisterHandler(typeof(S2C_ClientJoinMessage), DIContainer.Create<S2C_ClientJoinMessageHandler>());
            RegisterHandler(typeof(S2C_ClientConfirmMatchStateMessage), DIContainer.Create<S2C_ClientConfirmMatchStateMessageHandler>());
            // ...
        }

        private void RegisterHandler(Type msgType, IMessageHandler handler)
        {
            _handlers.Add(msgType, handler);
        }

        public void Dispatch(Message message)
        {
            if (!_handlers.TryGetValue(message.GetType(), out var handler)) 
                throw ExceptionHelper.Throw($"Handler {message.GetType()} not registered");
            
            // 处理器处理消息
            handler.HandleMessage(message);
        }
    }
}
