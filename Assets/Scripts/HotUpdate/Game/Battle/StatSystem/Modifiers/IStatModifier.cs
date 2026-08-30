using System;

namespace HotUpdate.Game.Battle.StatSystem.Modifiers
{
    /// <summary>
    /// 属性修改器接口
    /// </summary>
    public interface IStatModifier
    {
        /// <summary>
        /// 修改器ID
        /// </summary>
        long ModifierId { get; }
        
        /// <summary>
        /// 修正类型
        /// </summary>
        EModifierType ModifierType { get; }

        /// <summary>
        /// 修改器内容变化事件
        /// </summary>
        event Action OnChanged;
        
        /// <summary>
        /// 获取修改值，可以是固定值、百分比，转换的自定义值
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        float GetValue(StatEvaluationContext context);
    }
}
