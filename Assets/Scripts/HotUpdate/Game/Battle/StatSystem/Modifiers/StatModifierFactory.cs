using System;
using System.Collections.Generic;
using Core.Exceptions;

namespace HotUpdate.Game.Battle.StatSystem.Modifiers
{
    /// <summary>
    /// 属性修改器工厂，统一修改器对象获取、回收；新增修改器对象需新增对应方法；
    /// </summary>
    public class StatModifierFactory
    {
        // 修改器缓存
        private readonly Dictionary<EModifierType, Stack<IStatModifier>> _modifiers = new();
        
        public StatModifier Create(EModifierType statsModifierType, float value, out long modifierId)
        {
            StatModifier statModifier;
            if (_modifiers.TryGetValue(statsModifierType, out var statModifiers) && statModifiers.TryPop(out var modifier))
            {
                statModifier = (StatModifier)modifier;
                statModifier.Init(value);
                modifierId = statModifier.ModifierId;
                return statModifier;
            }
            
            statModifier = new StatModifier(statsModifierType);
            statModifier.Init(value);
            modifierId = statModifier.ModifierId;
            return statModifier;
        }

        public TestConversionModifier CreateConversion(EModifierType statsModifierType, StatSet sources, float ratio, float limit, out long modifierId)
        {
            TestConversionModifier statModifier;
            if (_modifiers.TryGetValue(statsModifierType, out var statModifiers) && statModifiers.TryPop(out var modifier))
            {
                statModifier = (TestConversionModifier)modifier;
                statModifier.Init(sources, ratio, limit);
                modifierId = statModifier.ModifierId;
                return statModifier;
            }
            
            statModifier = new TestConversionModifier(statsModifierType);
            statModifier.Init(sources, ratio, limit);
            modifierId = statModifier.ModifierId;
            return statModifier;
        }

        /// <summary>
        /// 回收修改器对象
        /// </summary>
        /// <param name="statModifier"></param>
        /// <exception cref="ArgumentNullException"><see cref="statModifier"/>为null时抛出</exception>
        public void Release(IStatModifier statModifier)
        {
            if(statModifier == null)
                throw ExceptionHelper.Throw<ArgumentNullException>(nameof(statModifier));
            
            if (_modifiers.TryGetValue(statModifier.ModifierType, out var statModifiers))
            {
                statModifiers.Push(statModifier);
            }
            else
            {
                var stack = new Stack<IStatModifier>();
                stack.Push(statModifier);
                _modifiers.Add(statModifier.ModifierType, stack);
            }
        }
        
    }
}
