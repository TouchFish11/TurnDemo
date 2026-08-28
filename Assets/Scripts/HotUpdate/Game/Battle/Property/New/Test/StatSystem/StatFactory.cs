using System;
using System.Collections.Generic;
using Core.Exceptions;
using HotUpdate.Game.Battle.Property.New.Test.StatSystem.Formulas;

namespace HotUpdate.Game.Battle.Property.New.Test.StatSystem
{
    public class StatFactory
    {
        private readonly Dictionary<EStatType, IStatFormula> _statFormulas = new();

        /// <summary>
        /// 默认白值计算公式
        /// </summary>
        public IStatFormula DefaultBaseValueFormula { get; } = new BaseFormula();
        
        /// <summary>
        /// 默认通用线性计算公式
        /// </summary>
        public IStatFormula DefaultLinearFormula { get; } = new BaseFormula();
        
        private StatFactory()
        {
            
        }

        /// <summary>
        /// 获取指定类型的计算公式
        /// </summary>
        /// <param name="statType"></param>
        /// <returns></returns>
        /// <exception cref="Exception">未找到类型的公式时抛出</exception>
        public IStatFormula GetStatFormula(EStatType statType)
        {
            return _statFormulas.GetValueOrDefault(statType) ?? throw ExceptionHelper.Throw(nameof(NullReferenceException));
        }
    }
}
