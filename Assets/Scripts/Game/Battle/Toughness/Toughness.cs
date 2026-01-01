using System.Collections.Generic;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 韧性
    /// </summary>
    public class Toughness
    {
        /// <summary>
        /// 弱点属性
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
        /// 是否已破盾
        /// </summary>
        public bool IsBroken => CurrentToughnessValue <= 0;

        public Toughness(List<E_ElementType> weakPropertys, int initialValue)
        {
            WeakPropertys = weakPropertys;
            CurrentToughnessValue = MaxToughnessVaue = initialValue;
        }

        /// <summary>
        /// 韧性削减
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="attackAttr"></param>
        public void ReduceToughness(int value)
        {
            // TODO：计算逻辑抽象为接口，便于拓展
            CurrentToughnessValue = Mathf.Max(0, CurrentToughnessValue - value);
        }
    }
}
