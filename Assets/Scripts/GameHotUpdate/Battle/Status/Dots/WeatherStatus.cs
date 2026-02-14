using Core.Config;
using Core.Pool;
using Core.Service;
using Core.Time;
using Game.Battle.Context;
using Game.Battle.Enum;
using Game.Battle.Objects;
using Game.Battle.Status;
using Game.VFX;
using GameHotUpdate.Battle.Damage.Data;
using GameHotUpdate.Battle.Event.General;
using GameHotUpdate.Battle.Event.UI;
using UnityEngine;

namespace GameHotUpdate.Battle.Status.Dots
{
    /// <summary>
    /// 风化
    /// </summary>
    [StatusTypeId(10004)]
    public class WeatherStatus : Status, IDotStatus
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
                Damage = 15,
                ElementType = E_ElementType.Wind,
                source = Sourcer,
                target = Owner
            };
            
            // 产生特效
            var vfxInfo = ServiceLocator.Get<IPoolManager>().GetData<VFXInfo>();
            var pos = Owner.GameObject.transform.position + Vector3.forward * 0.5f;
            pos = new Vector3(pos.x, 0.5f, pos.z);
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_Dot_Weather, 
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
