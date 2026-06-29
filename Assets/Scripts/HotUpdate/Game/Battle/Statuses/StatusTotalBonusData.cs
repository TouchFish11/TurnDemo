using System.Collections.Generic;

namespace HotUpdate.Game.Battle.Statuses
{
    /// <summary>
    /// 状态总加成数据
    /// </summary>
    public struct StatusTotalBonusData
    {
        /// <summary>
        /// 更新总攻击力加成
        /// </summary>
        /// <param name="statuses">状态集合</param>
        public void UpdateTotalAtkBonus(IEnumerable<IStatus> statuses)
        {
            TotalAtkPercentBonus = 0;
            TotalAtkBuildBonus = 0;

            foreach (var status in statuses)
            {
                // 累加攻击力百分比加成之和
                TotalAtkPercentBonus += status.BonusData.AtkPercentBonus;
                // 累加攻击力固定值加成之和
                TotalAtkBuildBonus += status.BonusData.AtkBuildBonus;
            }
        }

        /// <summary>
        /// 更新总防御力加成
        /// </summary>
        /// <param name="statuses">状态集合</param>
        public void UpdateTotalDefBonus(IEnumerable<IStatus> statuses)
        {
            TotalDefPercentBonus = 0;
            TotalDefBuildBonus = 0;

            foreach (var status in statuses)
            {
                // 累加防御力百分比加成之和
                TotalDefPercentBonus += status.BonusData.DefPercentBonus;
                // 累加防御力固定值加成之和
                TotalDefBuildBonus += status.BonusData.DefBuildBonus;
            }
        }

        /// <summary>
        /// 更新总生命值加成
        /// </summary>
        /// <param name="statuses">状态集合</param>
        public void UpdateTotalHpBonus(IEnumerable<IStatus> statuses)
        {
            TotalHpPercentBonus = 0;
            TotalHpBuildBonus = 0;

            foreach (var status in statuses)
            {
                // 累加生命值百分比加成之和
                TotalHpPercentBonus += status.BonusData.HpPercentBonus;
                // 累加生命值固定值加成之和
                TotalHpBuildBonus += status.BonusData.HpBuildBonus;
            }
        }

        // TODO：更新XXX加成
        // ...

        /// <summary>
        /// 总攻击力百分比加成
        /// </summary>
        public int TotalAtkPercentBonus { get; private set; }

        /// <summary>
        /// 总攻击力固定加成
        /// </summary>
        public int TotalAtkBuildBonus { get; private set; }

        /// <summary>
        /// 总防御力百分比加成
        /// </summary>
        public int TotalDefPercentBonus { get; private set; }

        /// <summary>
        /// 总防御力固定加成
        /// </summary>
        public int TotalDefBuildBonus { get; private set; }

        /// <summary>
        /// 总减防百分比
        /// </summary>
        public int TotalSubDefPercent { get; private set; }

        /// <summary>
        /// 总无视防御百分比
        /// </summary>
        public int TotalIgnoreDefPercent { get; private set; }

        /// <summary>
        /// 总生命值百分比加成
        /// </summary>
        public int TotalHpPercentBonus { get; private set; }

        /// <summary>
        /// 总生命值固定加成
        /// </summary>
        public int TotalHpBuildBonus { get; private set; }

        // TODO：总xxxx加成
        // ...
    }
}