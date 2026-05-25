using Core.DI;
using Core.Serialize.Binary;
using HotUpdate.Common.Config.ExcelInfo.Container;
using HotUpdate.Common.Config.ExcelInfo.Info;

namespace HotUpdate.Game.Battle.Status
{
    public class StatusProperty
    {
        [Inject] private IBinaryDataManager _binaryDataManager;
        
        /// <summary>
        /// 状态信息
        /// </summary>
        public StatusInfo StatusInfo { get; }

        // 剩余回合数
        private int remainingRound;
        // 当前层数
        private int currentPine;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="statusId">状态ID</param>
        public StatusProperty(int statusId)
        {
            StatusInfo = _binaryDataManager.GetConfig<StatusInfoContainer>(EConfigLoadType.Excel).dataDic[statusId];
            currentPine = StatusInfo.f_startPine;
            remainingRound = StatusInfo.f_durationRound;
        }

        /// <summary>
        /// 剩余回合数
        /// </summary>
        public int RemainingRound => remainingRound;

        /// <summary>
        /// 当前层数
        /// </summary>
        public int CurrentPine => currentPine;

        /// <summary>
        /// 设置剩余回合数
        /// </summary>
        /// <param name="remainingRound">剩余回合数</param>
        public void SetRemainingRound(int remainingRound)
        {
            this.remainingRound = remainingRound;
        }

        /// <summary>
        /// 设置当前层数
        /// </summary>
        /// <param name="currentPine">当前层数</param>
        public void SetCurrentPine(int currentPine)
        {
            this.currentPine = currentPine;
        }
    }
}