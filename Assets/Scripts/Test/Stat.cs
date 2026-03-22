using System;

namespace Test
{
    /// <summary>
    /// 属性
    /// </summary>
    public class Stat
    {
        /// <summary>
        /// 基础属性值
        /// </summary>
        public float BaseValue { get; set; }
        
        /// <summary>
        /// 最终属性值
        /// </summary>
        public float FinalValue { get; private set; }
    
        /// <summary>
        /// 属性值变化事件
        /// </summary>
        public event Action<float> OnValueChanged;
    
        /// <summary>
        /// 由加成管理器调用，传入当前最新的加成
        /// </summary>
        /// <param name="addedBonus">固定加成</param>
        /// <param name="percentBonus">百分比加成</param>
        public void UpdateValue(float addedBonus, float percentBonus) 
        {
            CalculateFinalValue(addedBonus, percentBonus);
            OnValueChanged?.Invoke(FinalValue);
        }

        private void CalculateFinalValue(float addedBonus, float percentBonus)
        {
            FinalValue = BaseValue * (1 + percentBonus) + addedBonus;
        }
    }
}
