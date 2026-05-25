using System.Collections.Generic;

namespace HotUpdate.Game.Battle.Status.Data
{
    /// <summary>
    /// 状态总加成数据
    /// </summary>
    public struct StatusTotalBonusData
    {
        // 总攻击力百分比加成
        private int _totalAtkPercentBonus;
        // 总攻击力固定加成
        private int _totalAtkBuildBonus;
        // 总防御力百分比加成
        private int _totalDefPercentBonus;
        // 总防御力固定加成
        private int _totalDefBuildBonus;
        // 总减防百分比
        private int _totalSubDefPercent;
        // 总无视防御百分比
        private int _totalIgnoreDefPercent;
        // 总生命值百分比加成
        private int _totalHpPercentBonus;
        // 总生命值固定加成
        private int _totalHpBuildBonus;

        /// <summary>
        /// 更新总攻击力加成
        /// </summary>
        /// <param name="statuses">状态集合</param>
        public void UpdateTotalAtkBonus(IEnumerable<IStatus> statuses)
        {
            _totalAtkPercentBonus = 0;
            _totalAtkBuildBonus = 0;

            foreach (IStatus status in statuses)
            {
                // 累加攻击力百分比加成之和
                _totalAtkPercentBonus += status.BonusData.AtkPercentBonus;
                // 累加攻击力固定值加成之和
                _totalAtkBuildBonus += status.BonusData.AtkBuildBonus;
            }
        }

        /// <summary>
        /// 更新总防御力加成
        /// </summary>
        /// <param name="statuses">状态集合</param>
        public void UpdateTotalDefBonus(IEnumerable<IStatus> statuses)
        {
            _totalDefPercentBonus = 0;
            _totalDefBuildBonus = 0;

            foreach (IStatus status in statuses)
            {
                // 累加防御力百分比加成之和
                _totalDefPercentBonus += status.BonusData.DefPercentBonus;
                // 累加防御力固定值加成之和
                _totalDefBuildBonus += status.BonusData.DefBuildBonus;
            }
        }

        /// <summary>
        /// 更新总生命值加成
        /// </summary>
        /// <param name="statuses">状态集合</param>
        public void UpdateTotalHpBonus(IEnumerable<IStatus> statuses)
        {
            _totalHpPercentBonus = 0;
            _totalHpBuildBonus = 0;

            foreach (IStatus status in statuses)
            {
                // 累加生命值百分比加成之和
                _totalHpPercentBonus += status.BonusData.HpPercentBonus;
                // 累加生命值固定值加成之和
                _totalHpBuildBonus += status.BonusData.HpBuildBonus;
            }
        }

        // 更新XXX加成
        // ...

        /// <summary>
        /// 总攻击力百分比加成
        /// </summary>
        public readonly int TotalAtkPercentBonus => _totalAtkPercentBonus;

        /// <summary>
        /// 总攻击力固定加成
        /// </summary>
        public readonly int TotalAtkBuildBonus => _totalAtkBuildBonus;

        /// <summary>
        /// 总防御力百分比加成
        /// </summary>
        public readonly int TotalDefPercentBonus => _totalDefPercentBonus;

        /// <summary>
        /// 总防御力固定加成
        /// </summary>
        public readonly int TotalDefBuildBonus => _totalDefBuildBonus;

        /// <summary>
        /// 总减防百分比
        /// </summary>
        public readonly int TotalSubDefPercent => _totalSubDefPercent;

        /// <summary>
        /// 总无视防御百分比
        /// </summary>
        public readonly int TotalIgnoreDefPercent => _totalIgnoreDefPercent;

        /// <summary>
        /// 总生命值百分比加成
        /// </summary>
        public readonly int TotalHpPercentBonus => _totalHpPercentBonus;

        /// <summary>
        /// 总生命值固定加成
        /// </summary>
        public readonly int TotalHpBuildBonus => _totalHpBuildBonus;

        // 总xxxx加成
        // ...
    }
}