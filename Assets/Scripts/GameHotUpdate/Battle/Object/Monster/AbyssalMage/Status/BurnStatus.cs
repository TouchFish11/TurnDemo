using Core.Pool;
using Core.Service;
using Core.Time;
using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Damage.Data;
using GameHotUpdate.Battle.Event.General;
using GameHotUpdate.Battle.Event.UI;
using GameHotUpdate.Battle.Property;
using GameHotUpdate.Battle.Status;
using GameHotUpdate.Config;
using GameHotUpdate.VFX;
using UnityEngine;

namespace GameHotUpdate.Battle.Object.Monster.AbyssalMage.Status
{
    /// <summary>
    /// 灼烧
    /// </summary>
    [StatusTypeId(1041)]
    public class BurnStatus : Battle.Status.Status, IDotStatus
    {
        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            StatusProperty.SetRemainingRound(StatusProperty.RemainingRound - 1);
            ApplyDamage();
        }
        
        private void ApplyDamage()
        {
            var damageCalcData = new DotDamageCalcData
            {
                Damage = 80,
                ElementType = E_ElementType.Wind,
                source = Sourcer,
                target = Owner
            };
            
            // 产生特效
            var vfxInfo = ServiceLocator.Get<IPoolManager>().GetData<VFXInfo>();
            var pos = Owner.GameObject.transform.position + Vector3.forward * 0.5f;
            pos = new Vector3(pos.x, 0.5f, pos.z);
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_Dot_Burn, 
                null, pos, Quaternion.identity, vfxInfo);

            ServiceLocator.Get<ITimerManager>().CreateTimer(false, 500, () =>
            {
                vfxInfo.IsStop = true;
                // 更新累计伤害UI
                Owner.Context.GetEventBus().TriggerEvent(new ClearCumulativeDamageEvent(Owner.Context));
            });
            
            Owner.Context.GetEventBus().TriggerEvent(new CalcDotDamageEvent(Owner.Context, damageCalcData));
        }
        
    }
}
