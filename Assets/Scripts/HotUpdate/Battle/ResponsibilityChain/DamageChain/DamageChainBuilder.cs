using HotUpdate.Battle.Damage.Data;

namespace HotUpdate.Battle.ResponsibilityChain.DamageChain
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
            var damageJudgeHandler = new DamageJudgeHandler();
            var shieldDefenseHandler = new ShieldDefenseHandler();
            var damageHandler = new DamageHandler();
            var nullDamageHandler = new NullDamageHandler();
            
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
            var damageJudgeHandler = new DamageJudgeHandler();
            var shieldDefenseHandler = new ShieldDefenseHandler();
            var toughnessHandler = new ToughnessHandler();
            var damageHandler = new DamageHandler();
            var nullDamageHandler = new NullDamageHandler();
            
            damageJudgeHandler.SetSuccessor(shieldDefenseHandler);
            shieldDefenseHandler.SetSuccessor(toughnessHandler);
            toughnessHandler.SetSuccessor(damageHandler);
            damageHandler.SetSuccessor(nullDamageHandler);
            
            return damageJudgeHandler;
        }
    }
}
