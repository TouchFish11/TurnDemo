using Core.Log;
using HotUpdate.Base.Animation;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Event.General;
using HotUpdate.Game.Battle.Property;

namespace HotUpdate.Game.Battle.ResponsibilityChain.DamageChain
{
    /// <summary>
    /// 伤害处理器
    /// </summary>
    public class DamageHandler : Handler<DamageResult>
    {
        public override void HandleRequest(DamageResult request)
        {
            if (request.Source == null || request.Target == null)
            {
                Logger.LogError(ELogTags.Battle, $"伤害处理异常," + $"Source:{request.Source},Target:{request.Target},技能ID:{request.SkillId}");
                successor.HandleRequest(request);
                return;
            }
            
            var target = request.Target;
            var context = request.Target.Context;
            // 执行应用伤害事件，显示伤害文本，显示护盾处理后的最终伤害
            context.EventBus.TriggerEvent(new ApplyDamageEvent(context, request));
            
            // 播放受击动画
            target.GetComponent<BattleAnimationComponent>().Play(EAnimationType.Hit);
            // 获取属性组件，处理血量扣减
            var propertyComponent = target.GetComponent<PropertyComponent>();
            // 获取当前血量
            var currentHp = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp);
            // 扣减最终伤害量
            propertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentHp, currentHp - request.FinalDamage);
            // 修正血量：最小为0（防止血量为负数）
            currentHp = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp);
            if (currentHp <= 0)
            {
                propertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentHp, 0);
            }
        }
    }
}
