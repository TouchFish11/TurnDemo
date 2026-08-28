using System.Collections.Generic;

namespace HotUpdate.Game.Battle.Property.New.Test.StatSystem
{
    /// <summary>
    /// 属性计算上下文
    /// </summary>
    public struct StatEvaluationContext
    {
        /// <summary>
        /// 当前固定值总和
        /// </summary>
        public float Flat { get; set; }
        
        /// <summary>
        /// 当前百分比值总和，小数表示
        /// </summary>
        public float Percent { get; set; }
        
        /// <summary>
        /// 基础值
        /// </summary>
        public float BaseValue { get; }
        
        /// <summary>
        /// 修改器集合
        /// </summary>
        public IEnumerable<IStatModifier> Modifiers { get; }
        
        /// <summary>
        /// 当前依赖链
        /// </summary>
        public Stack<IStatModifier> Denpendencys { get; }

        public StatEvaluationContext(float baseValue,  IEnumerable<IStatModifier> modifiers)
        {
            BaseValue = baseValue;
            Modifiers = modifiers;
            Denpendencys = new Stack<IStatModifier>();
            Flat = 0;
            Percent = 0;
        }
    }
}
