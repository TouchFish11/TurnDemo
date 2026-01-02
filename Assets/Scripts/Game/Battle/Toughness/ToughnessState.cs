using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Game.Battle
{
    /// <summary>
    /// 韧性状态
    /// </summary>
    public class ToughnessState
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
        /// 是否已击破
        /// </summary>
        public bool IsBroken => CurrentToughnessValue <= 0;

        public ToughnessState(List<E_ElementType> weakPropertys, int initialValue)
        {
            WeakPropertys = weakPropertys;
            CurrentToughnessValue = MaxToughnessVaue = initialValue;
        }

        /// <summary>
        /// 设置韧性值
        /// </summary>
        /// <param name="current"></param>
        /// <param name="max"></param>
        public void SetToughnessValue(int current, int max)
        {
            CurrentToughnessValue = current;
            MaxToughnessVaue = max;
        }
    }
}
