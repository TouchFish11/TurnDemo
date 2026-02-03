using Game.Battle.Damage;

namespace GameHotUpdate.Battle.ResponsibilityChain.DamageChain
{
    /// <summary>
    /// 伤害链构建器
    /// </summary>
    public static class DamageChainBuilder
    {
        public static Handler<DamageResult> GetRolrDamageChain()
        {
            var damageJudgeHandler = new DamageJudgeHandler();
            var damageHandler = new DamageHandler();
            var nullDamageHandler = new NullDamageHandler();
            
            damageJudgeHandler.SetSuccessor(damageHandler);
            damageHandler.SetSuccessor(nullDamageHandler);
            
            return damageJudgeHandler;
        }
        
        public static Handler<DamageResult> GetMonsterDamageChain()
        {
            var damageJudgeHandler = new DamageJudgeHandler();
            var toughnessHandler = new ToughnessHandler();
            var damageHandler = new DamageHandler();
            var nullDamageHandler = new NullDamageHandler();
            
            damageJudgeHandler.SetSuccessor(toughnessHandler);
            toughnessHandler.SetSuccessor(damageHandler);
            damageHandler.SetSuccessor(nullDamageHandler);
            
            return damageJudgeHandler;
        }
    }
}
