using System;
using Core.Exceptions;

namespace HotUpdate.Game.Battle.Property.New.Test.StatSystem.Formulas
{
    /// <summary>
    /// 白值计算公式
    /// </summary>
    public class BaseFormula : IStatFormula
    {
        public float Calculate(StatEvaluationContext context)
        {
            foreach (var modifier in context.Modifiers)
            {
                switch (modifier.ModifierType)
                {
                    case EModifierType.Flat:
                        context.Flat += modifier.GetValue(context);
                        break;
                    case EModifierType.Percent:
                        context.Percent += modifier.GetValue(context);
                        break;
                    case EModifierType.Conversion:
                        modifier.GetValue(context);
                        break;
                    default:
                        throw ExceptionHelper.Throw(nameof(ArgumentOutOfRangeException));
                }
            }
            
            return context.Flat * (1 + context.Percent);
        }
    }
}
