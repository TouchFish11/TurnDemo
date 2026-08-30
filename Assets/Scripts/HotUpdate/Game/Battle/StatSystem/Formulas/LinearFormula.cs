using System;
using Core.Exceptions;

namespace HotUpdate.Game.Battle.StatSystem.Formulas
{
    /// <summary>
    /// 通用线性计算公式
    /// </summary>
    public class LinearFormula : IStatFormula
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
            
            return context.BaseValue * (1 + context.Percent) + context.Flat;
        }
    }
}
