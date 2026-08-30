namespace HotUpdate.Game.Battle.Statuses
{
    /// <summary>
    /// 状态运行时属性
    /// </summary>
    public class StatusProperty
    {
        /// <summary>
        /// 剩余回合数
        /// </summary>
        public int RemainingRound { get; set; }

        /// <summary>
        /// 当前层数
        /// </summary>
        public int CurrentPine { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="statusInfo"></param>
        public StatusProperty(StatusInfo statusInfo)
        {
            CurrentPine = statusInfo.f_startPine;
            RemainingRound = statusInfo.f_durationRound;
        }
    }
}