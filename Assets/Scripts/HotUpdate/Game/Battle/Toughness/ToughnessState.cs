using System.Collections.Generic;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Property;

namespace HotUpdate.Game.Battle.Toughness
{
    /// <summary>
    /// 韧性状态
    /// </summary>
    public class ToughnessState
    {
        /// <summary>
        /// 弱点属性列表
        /// </summary>
        public List<E_ElementType> WeakPropertys { get; private set; }

        /// <summary>
        /// 当前韧性值
        /// </summary>
        public int CurrentToughnessValue { get; private set; }

        /// <summary>
        /// 最大韧性值
        /// </summary>
        public int MaxToughnessVaue { get; private set; }

        /// <summary>
        /// 是否已击破
        /// </summary>
        public bool IsBroken => CurrentToughnessValue <= 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="weakPropertys">弱点属性列表</param>
        /// <param name="initialValue">初始韧性值</param>
        public ToughnessState(List<E_ElementType> weakPropertys, int initialValue)
        {
            WeakPropertys = weakPropertys;
            CurrentToughnessValue = MaxToughnessVaue = initialValue;
        }

        /// <summary>
        /// 设置韧性值
        /// </summary>
        /// <param name="current">当前韧性值</param>
        /// <param name="max">最大韧性值</param>
        public void SetToughnessValue(int current, int max)
        {
            CurrentToughnessValue = current;
            MaxToughnessVaue = max;
        }
    }
}