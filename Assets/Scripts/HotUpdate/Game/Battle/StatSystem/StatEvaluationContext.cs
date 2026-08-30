using System.Collections.Generic;
using HotUpdate.Game.Battle.StatSystem.Modifiers;

namespace HotUpdate.Game.Battle.StatSystem
{
    /// <summary>
    /// 属性计算上下文
    /// </summary>
    public struct StatEvaluationContext
    {
        /// <summary>
        /// 属性配置底值
        /// </summary>
        public float SeedValue { get; }
        
        /// <summary>
        /// 当前固定值总和
        /// </summary>
        public float Flat { get; set; }
        
        /// <summary>
        /// 当前百分比值总和，小数表示
        /// </summary>
        public float Percent { get; set; }
        
        /// <summary>
        /// 实际基础值
        /// </summary>
        public float BaseValue { get; }
        
        /// <summary>
        /// 修改器集合
        /// </summary>
        public IEnumerable<IStatModifier> Modifiers { get; }
        
        /// <summary>
        /// 当前依赖链
        /// </summary>
        public static Stack<IStatModifier> Dependencies { get; } = new();

        public StatEvaluationContext(float seedValue, float baseValue, IEnumerable<IStatModifier> modifiers)
        {
            SeedValue = seedValue;
            BaseValue = baseValue;
            Modifiers = modifiers;
            Flat = 0;
            Percent = 0;
        }
    }
}
