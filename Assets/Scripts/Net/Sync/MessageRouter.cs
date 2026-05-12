using System;
using System.Collections.Generic;
using Core.DI;
using Net.Sync.Handlers;
using Net.Sync.Msg.S2C;

namespace Net.Sync
{
    /// <summary>
    /// 消息路由器
    /// </summary>
    public class MessageRouter
    {
        private readonly Dictionary<Type, IMessageHandler> _handlers = new();

        public MessageRouter()
        {
            RegisterHandler(typeof(ConnectMessage), DIContainer.Create<ConnectMessageHandler>());
            // ...
        }

        private void RegisterHandler(Type msgType, IMessageHandler handler)
        {
            _handlers.Add(msgType, handler);
        }

        public void Dispatch(Message message)
        {
            if (!_handlers.TryGetValue(message.GetType(), out var handler)) throw new Exception($"Handler {message.GetType()} not registered");
            // 处理器处理消息
            handler.HandleMessage(message);
        }
    }
}
