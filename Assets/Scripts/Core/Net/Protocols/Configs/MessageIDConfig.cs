namespace Net.Protocols.Configs
{
    /// <summary>
    /// 消息ID配置，定义所有有自定义消息头的ID
    /// </summary>
    public static class MessageIDConfig
    {
        /// <summary>
        /// 新客户端加入后，服务器发送给其它客户端的消息ID
        /// </summary>
        public const int S2C_ClientJoin_ID = 1001;
        
        /// <summary>
        /// 服务器到客户端的连接消息ID，携带当前客户端的唯一ID和其它客户端的ID
        /// </summary>
        public const int S2C_Connect_ID = 1002;
        
        /// <summary>
        /// 请求断开连接消息ID
        /// 客户端主动断开则发送给服务器作为断开请求；
        /// </summary>
        public const int C2S_RequestDisconnect_ID = 1003;
        
        /// <summary>
        /// 断开连接消息ID
        /// 客户端收到服务器则可以真正断开
        /// </summary>
        public const int S2C_Disconnect_ID = 1004;
        
        /// <summary>
        /// 心跳消息ID
        /// 客户端和服务器来回发送的消息，由客户端先发送
        /// </summary>
        public const int Heartbeat_ID = 1005;
        
        /// <summary>
        /// 客户端请求匹配匹配消息ID
        /// </summary>
        public const int C2S_Match_ID = 2001;
        
        /// <summary>
        /// 匹配成功消息ID
        /// </summary>
        public const int S2C_MatchSuccess_ID = 2002;
        
        /// <summary>
        /// 匹配确认消息
        /// </summary>
        public const int C2S_MatchConfirm_ID = 2003;
        
        /// <summary>
        /// 客户端准备比赛消息ID
        /// </summary>
        public const int S2C_PrepareRace_ID = 2004;
        
        /// <summary>
        /// 离开比赛消息ID
        /// </summary>
        public const int LeaveRace_ID = 2005;
        
        /// <summary>
        /// 开始比赛消息ID
        /// </summary>
        public const int S2C_StartRace_ID = 2006;
        
        /// <summary>
        /// 客户端比赛准备完毕消息ID
        /// </summary>
        public const int C2S_Ready_ID = 2007;
        
        /// <summary>
        /// 客户端比赛重连消息ID
        /// </summary>
        public const int ReconnectRace_ID = 2008;
        
        /// <summary>
        /// 通知其它客户端某个客户端确认状态的消息
        /// </summary>
        public const int S2C_Notice_MatchConfirmState_ID = 2009;
        
        /// <summary>
        /// 客户端发送给服务器的下一帧消息的ID
        /// </summary>
        public const int C2S_NextFrame_ID = 3001;

        /// <summary>
        /// 服务器发送给客户端的下一帧消息的ID
        /// </summary>
        public const int S2C_Frame_ID = 3002;
    }
}
