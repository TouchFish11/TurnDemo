using GameLogic.BattleMoudule.Entity;
using System.Collections.Generic;

namespace Game.Battle
{
    /// <summary>
    /// 韧性（破盾）相关定义
    /// </summary>
    public class Toughness
    {
        /// <summary>
        /// 弱点属性（如物理、风）
        /// </summary>
        public List<E_PropertyType> WeakPropertys { get; }

        /// <summary>
        /// 当前韧性值
        /// </summary>
        public float CurrentValue { get; private set; }

        /// <summary>
        /// 是否已破盾
        /// </summary>
        public bool IsBroken => CurrentValue <= 0;

        public Toughness(List<E_PropertyType> weakPropertys, float initialValue)
        {
            WeakPropertys = weakPropertys;
            CurrentValue = initialValue;
        }

        /// <summary>
        /// 韧性削减（仅内部/授权模块调用）
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="attackAttr"></param>
        public void ReduceToughness(E_PropertyType propertyType, float value)
        {
            // 弱点属性伤害翻倍
            //var finalDamage = attackAttr == WeakAttribute ? damage * 2 : damage;
            //CurrentValue = Math.Max(0, CurrentValue - finalDamage);
            //Console.WriteLine($"韧性值变化：{CurrentValue + finalDamage} → {CurrentValue}（{propertyType}属性攻击）");
        }
    }
}
