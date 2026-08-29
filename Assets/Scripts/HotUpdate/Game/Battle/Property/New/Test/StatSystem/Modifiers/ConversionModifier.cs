using Core.Exceptions;

namespace HotUpdate.Game.Battle.Property.New.Test.StatSystem.Modifiers
{
    public abstract class ConversionModifier : StatModifier
    {
        protected readonly StatSet source;

        protected ConversionModifier(StatSet sources) : base(EModifierType.Conversion, 0)
        {
            source = sources;
        }
    
        public override float GetValue(StatEvaluationContext context)
        {
            if (context.Denpendencys.Contains(this))
                throw ExceptionHelper.Throw($"Stat evaluation fail({nameof(ModifierId)}:{ModifierId}), denpendency:({string.Join("->", context.Denpendencys)})");
            
            context.Denpendencys.Push(this);
            var value = OnGetValue(context);
            context.Denpendencys.Pop();
            return value;
        }
        
        /// <summary>
        /// 自定义转换逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract float OnGetValue(StatEvaluationContext context);
    }
}
