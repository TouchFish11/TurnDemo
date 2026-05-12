namespace HotUpdate.Base.Battle.Status.Data
{
    public struct StatusBonusData
    {
        /// <summary>
        /// �������ٷֱȼӳ�����
        /// </summary>
        public int AtkPercentBonus { get; set; }

        /// <summary>
        /// �������̶���ֵ�ӳ�����
        /// </summary>
        public int AtkBuildBonus { get; set; }

        /// <summary>
        /// �������ٷֱȼӳ�����
        /// </summary>
        public int DefPercentBonus { get; set; }

        /// <summary>
        /// �������̶���ֵ�ӳ�����
        /// </summary>
        public int DefBuildBonus { get; set; }

        /// <summary>
        /// ���ͷ������ٷֱ�
        /// </summary>
        public int SubDefPercent { get; set; }

        /// <summary>
        /// ���ӷ����ٷֱ�
        /// </summary>
        public int IgnoreDefPercent { get; set; }

        /// <summary>
        /// ����ֵ�ٷֱȼӳ�����
        /// </summary>
        public int HpPercentBonus { get; set; }

        /// <summary>
        /// ����ֵ�̶���ֵ�ӳ�����
        /// </summary>
        public int HpBuildBonus { get; set; }
    }
}
