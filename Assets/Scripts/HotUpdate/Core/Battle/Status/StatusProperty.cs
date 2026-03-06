using Core.Serialize.Binary;
using Core.Service;

namespace HotUpdate.Core.Battle.Status
{
    public class StatusProperty
    {
        /// <summary>
        /// 
        /// </summary>
        public StatusInfo StatusInfo { get; }

        //
        private int remainingRound; //
        private int currentPine;    //

        public StatusProperty(int statusId)
        {
            StatusInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<StatusInfoContainer>(EConfigLoadType.Excel).dataDic[statusId];
            currentPine = StatusInfo.f_startPine;
            remainingRound = StatusInfo.f_durationRound;
        }

        /// <summary>
        /// ʣ��غ�
        /// </summary>
        public int RemainingRound { get => remainingRound; }
        /// <summary>
        /// ��ǰ����
        /// </summary>
        public int CurrentPine { get => currentPine; }

        /// <summary>
        /// ����ʣ��غ���
        /// </summary>
        /// <param name="remainingRound"></param>
        public void SetRemainingRound(int remainingRound)
        {
            this.remainingRound = remainingRound;
        }

        /// <summary>
        /// ���õ�ǰ����
        /// </summary>
        /// <param name="currentPine"></param>
        public void SetCurrentPine(int currentPine)
        {
            this.currentPine = currentPine;
        }
    }
}
