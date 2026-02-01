using System.Collections.Generic;

namespace Game.Battle.Status.Data
{
    /// <summary>
    /// ״̬�ܼӳ�����
    /// </summary>
    public struct StatusTotalBonusData
    {
        // �ܹ������ٷֱȼӳ�
        private int _totalAtkPercentBonus;
        // �ܹ������̶��ӳ�
        private int _totalAtkBuildBonus;
        // �ܷ������ٷֱȼӳ�
        private int _totalDefPercentBonus;
        // �ܷ������̶��ӳ�
        private int _totalDefBuildBonus;
        // �ܼ����ٷֱ�
        private int _totalSubDefPercent;
        // �����ӷ����ٷֱ�
        private int _totalIgnoreDefPercent;
        // ������ֵ�ٷֱȼӳ�
        private int _totalHpPercentBonus;
        // ������ֵ�̶��ӳ�
        private int _totalHpBuildBonus;

        /// <summary>
        /// �����ܹ������ӳ�
        /// </summary>
        public void UpdateTotalAtkBonus(IEnumerable<IStatus> statuses)
        {
            _totalAtkPercentBonus = 0;
            _totalAtkBuildBonus = 0;

            foreach (IStatus status in statuses)
            {
                // ���㹥�����ٷֱȼӳ�֮��
                _totalAtkPercentBonus += status.BonusData.AtkPercentBonus;
                // ���㹥���̶���ֵ�ӳ�֮��
                _totalAtkBuildBonus += status.BonusData.AtkBuildBonus;
            }
        }

        /// <summary>
        /// �����ܷ������ӳ�
        /// </summary>
        public void UpdateTotalDefBonus(IEnumerable<IStatus> statuses)
        {
            _totalDefPercentBonus = 0;
            _totalDefBuildBonus = 0;

            foreach (IStatus status in statuses)
            {
                // ����������ٷֱȼӳ�֮��
                _totalDefPercentBonus += status.BonusData.DefPercentBonus;
                // ��������̶���ֵ�ӳ�֮��
                _totalDefBuildBonus += status.BonusData.DefBuildBonus;
            }
        }

        /// <summary>
        /// ����������ֵ�ӳ�
        /// </summary>
        public void UpdateTotalHpBonus(IEnumerable<IStatus> statuses)
        {
            _totalHpPercentBonus = 0;
            _totalHpBuildBonus = 0;

            foreach (IStatus status in statuses)
            {
                // ��������ֵ�ٷֱȼӳ�֮��
                _totalHpPercentBonus += status.BonusData.HpPercentBonus;
                // ��������ֵ�̶���ֵ�ӳ�֮��
                _totalHpBuildBonus += status.BonusData.HpBuildBonus;
            }
        }

        // ������XXX�ӳ�
        // ...

        /// <summary>
        /// �ܹ������ٷֱȼӳ�
        /// </summary>
        public readonly int TotalAtkPercentBonus { get => _totalAtkPercentBonus; }

        /// <summary>
        /// �ܹ������̶��ӳ�
        /// </summary>
        public readonly int TotalAtkBuildBonus { get => _totalAtkBuildBonus; }

        /// <summary>
        /// �ܷ������ٷֱȼӳ�
        /// </summary>
        public readonly int TotalDefPercentBonus { get => _totalDefPercentBonus; }

        /// <summary>
        /// �ܷ������̶��ӳ�
        /// </summary>
        public readonly int TotalDefBuildBonus { get => _totalDefBuildBonus; }

        /// <summary>
        /// �ܼ����ٷֱ�
        /// </summary>
        public readonly int TotalSubDefPercent { get => _totalSubDefPercent; }

        /// <summary>
        /// �����ӷ����ٷֱ�
        /// </summary>
        public readonly int TotalIgnoreDefPercent { get => _totalIgnoreDefPercent; }

        /// <summary>
        /// ������ֵ�ٷֱȼӳ�
        /// </summary>
        public readonly int TotalHpPercentBonus { get => _totalHpPercentBonus; }

        /// <summary>
        /// ������ֵ�̶��ӳ�
        /// </summary>
        public readonly int TotalHpBuildBonus { get => _totalHpBuildBonus; }

        // ��xxxx�ӳ�
        // ...
    }
}
