using HotUpdate.Common.Config.ExcelInfo.Info;

namespace HotUpdate.Game.Battle.Statuses
{
    /// <summary>
    /// 状态熟悉
    /// </summary>
    public class StatusProperty
    {
        /// <summary>
        /// 状态信息
        /// </summary>
        public StatusInfo StatusInfo { get; }
        
        /// <summary>
        /// 剩余回合数
        /// </summary>
        public int RemainingRound { get; private set; }

        /// <summary>
        /// 当前层数
        /// </summary>
        public int CurrentPine { get; private set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="statusInfo"></param>
        public StatusProperty(StatusInfo statusInfo)
        {
            CurrentPine = statusInfo.f_startPine;
            RemainingRound = statusInfo.f_durationRound;
            StatusInfo = statusInfo;
        }

        /// <summary>
        /// 设置剩余回合数
        /// </summary>
        /// <param name="remainingRound">剩余回合数</param>
        public void SetRemainingRound(int remainingRound)
        {
            RemainingRound = remainingRound;
        }

        /// <summary>
        /// 设置当前层数
        /// </summary>
        /// <param name="currentPine">当前层数</param>
        public void SetCurrentPine(int currentPine)
        {
            CurrentPine = currentPine;
        }
    }
}