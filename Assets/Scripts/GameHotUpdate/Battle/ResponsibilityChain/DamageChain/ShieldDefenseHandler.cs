using Game.Battle.Damage;
using Game.Battle.Enum;
using GameHotUpdate.Battle.Property;

namespace GameHotUpdate.Battle.ResponsibilityChain.DamageChain
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
            var propertyComponent = target.GetComponent<PropertyComponent>();
            // 获取当前护盾
            var currentShield = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentShield);
            // 存在护盾
            if (currentShield > 0)
            {
                // 新最后伤害
                var newFinalDmg = request.FinalDamage - currentShield;
                // 伤害大于护盾量
                if (newFinalDmg > 0)
                {
                    // 护盾为0
                    propertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentEnergy, 0);
                    // 传递剩余伤害
                    var damageResult = new DamageResult
                    (
                        source: request.Source,
                        target: request.Target,
                        finalDamage: newFinalDmg,
                        elementType: request.ElementType,
                        damageType: request.DamageType,
                        isCrit: request.IsCrit, skillId: request.SkillId, resilienceValue: request.ResilienceValue);
                    
                    successor.HandleRequest(damageResult);
                }
                else
                {
                    // 更新剩余护盾
                    propertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentShield, currentShield - request.FinalDamage);
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
