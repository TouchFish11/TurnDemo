using Core.Log;
using Game.Battle.Damage;

namespace GameHotUpdate.Battle.ResponsibilityChain.DamageChain
{
    /// <summary>
    /// 伤害判断处理器
    /// </summary>
    public class DamageJudgeHandler : Handler<DamageResult>
    {
        public override void HandleRequest(DamageResult request)
        {
            if (!CanTakeDamage(request))
            {
                LogManager.Log($"{nameof(DamageJudgeHandler)}.{nameof(HandleRequest)}：不可受伤");
                return;
            }
            
            successor.HandleRequest(request);
        }
        
        /// <summary>
        /// 判定是否可承受伤害
        /// （如无敌、免疫状态下返回false）
        /// </summary>
        /// <returns>true=可承受伤害，false=不可承受伤害</returns>
        protected bool CanTakeDamage(DamageResult request)
        {
            return true;
        }
    }
}
