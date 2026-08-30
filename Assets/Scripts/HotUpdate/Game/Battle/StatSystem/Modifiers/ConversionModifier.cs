using System;
using Core.Exceptions;

namespace HotUpdate.Game.Battle.StatSystem.Modifiers
{
    /// <summary>
    /// 属性转换修改器
    /// </summary>
    public abstract class ConversionModifier : StatModifier
    {
        protected StatSet source;

        protected ConversionModifier(EModifierType modifierType) : base(modifierType)
        {

        }

        public override float GetValue(StatEvaluationContext context)
        {
            if (StatEvaluationContext.Dependencies.Contains(this))
                throw ExceptionHelper.Throw($"Stat evaluation fail({nameof(ModifierId)}:{ModifierId}), denpendency:({string.Join("->", StatEvaluationContext.Dependencies)})");
            
            StatEvaluationContext.Dependencies.Push(this);
            try
            {
                return OnGetValue(context);
            }
            finally
            {
                StatEvaluationContext.Dependencies.Pop();
            }
        }
        
        /// <summary>
        /// 自定义转换逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract float OnGetValue(StatEvaluationContext context);
    }
}
