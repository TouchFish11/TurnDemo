using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Property;

namespace HotUpdate.Game.Battle.ResponsibilityChain.DamageChain
{
    /// <summary>
    /// 护盾抵挡处理器
    /// </summary>
    public class ShieldDefenseHandler : Handler<DamageResult>
    {
        public override void HandleRequest(DamageResult request)
        {
            var target = request.Target;
            // 获取属性组件，处理护盾削减
            var statsComponent = target.GetComponent<StatsComponent>();
            // 获取当前护盾
            var currentShield = statsComponent.CurrentShiled;
            // 存在护盾
            if (currentShield > 0)
            {
                // 新的最后伤害
                var newFinalDmg = request.FinalDamage - currentShield;
                // 伤害大于护盾量
                if (newFinalDmg > 0)
                {
                    // 护盾为0
                    statsComponent.UpdateShield(-statsComponent.CurrentShiled);
                    // 传递剩余伤害
                    var damageResult = new DamageResult
                    (
                        source: request.Source,
                        target: request.Target,
                        finalDamage: (int)newFinalDmg,
                        elementType: request.ElementType,
                        damageType: request.DamageType,
                        isCrit: request.IsCrit, skillId: request.SkillId, resilienceValue: request.ResilienceValue);
                    
                    successor.HandleRequest(damageResult);
                }
                else
                {
                    // 更新剩余护盾
                    statsComponent.UpdateShield(-request.FinalDamage);
                    return;
                }
            }
            else
            {
                successor.HandleRequest(request);
            }
        }
    }
}
