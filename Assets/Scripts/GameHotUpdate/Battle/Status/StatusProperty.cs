using Core.Serialize.Binary;
using Core.Service;

namespace GameHotUpdate.Battle.Status
{
    /// <summary>
    /// ״̬����
    /// </summary>
    public class StatusProperty
    {
        /// <summary>
        /// ״̬��Ϣ
        /// </summary>
        public StatusInfo StatusInfo { get; }

        // ��̬����
        private int remainingRound; // ʣ��غ�
        private int currentPine;    // ��ǰ����

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
