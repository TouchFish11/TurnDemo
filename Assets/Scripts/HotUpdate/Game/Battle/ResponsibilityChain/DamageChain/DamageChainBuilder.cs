using Core.DI;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Damage;

namespace HotUpdate.Game.Battle.ResponsibilityChain.DamageChain
{
    /// <summary>
    /// 伤害链构建器
    /// </summary>
    public static class DamageChainBuilder
    {
        /// <summary>
        /// 构建角色处理伤害链
        /// </summary>
        /// <returns></returns>
        public static Handler<DamageResult> GetRoleDamageChain()
        {
            var damageJudgeHandler = DIContainer.Create<DamageJudgeHandler>();
            var shieldDefenseHandler = DIContainer.Create<ShieldDefenseHandler>();
            var damageHandler = DIContainer.Create<DamageHandler>();
            var nullDamageHandler = DIContainer.Create<NullDamageHandler>();
            
            damageJudgeHandler.SetSuccessor(shieldDefenseHandler);
            shieldDefenseHandler.SetSuccessor(damageHandler);
            damageHandler.SetSuccessor(nullDamageHandler);
            
            return damageJudgeHandler;
        }
        
        /// <summary>
        /// 构建怪物处理伤害链
        /// </summary>
        /// <returns></returns>
        public static Handler<DamageResult> GetMonsterDamageChain()
        {
            var damageJudgeHandler = DIContainer.Create<DamageJudgeHandler>();
            var shieldDefenseHandler = DIContainer.Create<ShieldDefenseHandler>();
            var toughnessHandler = DIContainer.Create<ToughnessHandler>();
            var damageHandler =  DIContainer.Create<DamageHandler>();
            var nullDamageHandler =  DIContainer.Create<NullDamageHandler>();
            
            damageJudgeHandler.SetSuccessor(shieldDefenseHandler);
            shieldDefenseHandler.SetSuccessor(toughnessHandler);
            toughnessHandler.SetSuccessor(damageHandler);
            damageHandler.SetSuccessor(nullDamageHandler);
            
            return damageJudgeHandler;
        }
    }
}
