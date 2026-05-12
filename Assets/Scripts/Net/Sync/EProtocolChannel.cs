namespace Net.Sync
{
    /// <summary>
    /// 协议通道
    /// </summary>
    public enum EProtocolChannel : byte
    {
        /// <summary>
        /// 可靠的
        /// </summary>
        Reliable   = 1,
        
        /// <summary>
        /// 不可靠的
        /// </summary>
        Unreliable = 2
    }
}
