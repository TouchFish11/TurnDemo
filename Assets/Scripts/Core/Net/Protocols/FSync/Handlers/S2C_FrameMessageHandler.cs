using System.Collections.Generic;
using Core.DI;
using Core.Net.SyncModule.Interface;
using Core.Net.SyncModule.Manager;
using Net.Protocols;
using Net.Protocols.FSync.Messages;

namespace Core.Net.Protocols.FSync.Handlers
{
    /// <summary>
    /// 帧消息处理器，处理接收到的服务器帧消息
    /// </summary>
    public class S2C_FrameMessageHandler : MessageHandler<S2C_FrameMessage>
    {
        [Inject] private NetGameManager _netGameManager;
        [Inject] private INetManager _netManager;
        [Inject] private NetGameProxy _proxy;
        
        public override S2C_FrameMessage Message { get; protected set; }
        
        /// <summary>
        /// 已经执行到的帧号。初始 -1，这样第 0 帧能被正确执行。
        /// </summary>
        public int FrameId { get; set; } = -1;
        
        // 帧缓冲：帧ID -> 一帧所有输入。TCP 是有序到达，这里主要做「暂存 + 按序取」
        private readonly SortedDictionary<int, S2C_OneFrameMessage> _frameBuffer = new();
        
        /// <summary>
        /// 逻辑帧时间
        /// </summary>
        private const float LogicTime = 1 / 15f;
        
        protected override void OnHandle()
        {
            InputServerCommand(Message);
        }
        
        /// <summary>
        /// 收到服务器广播的帧数据
        /// </summary>
        public void InputServerCommand(S2C_FrameMessage s2CFrameMessage)
        {
            // 把收到的所有帧放进缓冲
            foreach (var frame in s2CFrameMessage.FrameMessages)
            {
                _frameBuffer[frame.FrameID] = frame;
            }

            // 从「已执行帧 + 1」开始，只要缓冲里有，就按顺序执行
            while (_frameBuffer.TryGetValue(FrameId + 1, out var nextFrame))
            {
                _frameBuffer.Remove(FrameId + 1);
                ExecuteFrame(nextFrame);
                FrameId++;
            }

            // 执行完，采集本地输入，上报给服务器（为下一帧准备）
            SendFrameInput();
        }
        
        private void ExecuteFrame(S2C_OneFrameMessage frame)
        {
            foreach (var optMessage in frame.OptMessages)
            {
                if (_netGameManager.TryGetPlayer(optMessage.SessionID, out var netObject))
                {
                    netObject.SyncFrame(optMessage);
                }
            }
        }
        
        /// <summary>
        /// 采集本地输入并发送给服务器
        /// </summary>
        private void SendFrameInput()
        {
            var c2SNextFrameMessage = new C2S_NextFrameMessage
            {
                FrameID = FrameId + 1,
                OptMessage = new OptMessage
                {
                    SessionID = _netManager.SessionId
                }
            };

            if (_netGameManager.TryGetPlayer(_netManager.SessionId, out var netObject))
            {
                // 采集本地输入
                netObject.CollectInput(c2SNextFrameMessage.OptMessage);
                // 发送
                _proxy.Send(c2SNextFrameMessage, EProtocolChannel.Raw);
            }
        }
    }
}
