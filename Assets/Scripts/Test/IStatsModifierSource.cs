using System;
using System.Collections.Generic;

namespace Test
{
    /// <summary>
    /// 属性加成修改器源接口
    /// </summary>
    public interface IStatsModifierSource
    {
        /// <summary>
        /// 属性加成修改器变化事件
        /// </summary>
        event Action OnModifiersChanged;

        /// <summary>
        /// 获取最新的属性加成集合
        /// </summary>
        /// <param name="bonusDatas"></param>
        /// <returns></returns>
        void GetModifier(Dictionary<EStatType, BonusData> bonusDatas);
    }
}
