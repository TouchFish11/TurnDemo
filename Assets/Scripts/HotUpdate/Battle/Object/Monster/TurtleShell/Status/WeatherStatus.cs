using Core.Pool;
using Core.Service;
using Core.Time;
using HotUpdate.Battle.Event.General;
using HotUpdate.Battle.Event.UI;
using HotUpdate.Battle.Property;
using HotUpdate.Battle.Status;
using HotUpdate.Common;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Damage.Data;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.VFX;
using UnityEngine;

namespace HotUpdate.Battle.Object.Monster.TurtleShell.Status
{
    /// <summary>
    /// 风化
    /// </summary>
    [StatusTypeId(1021)]
    public class WeatherStatus : Battle.Status.Status, IDotStatus
    {
        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            StatusProperty.SetRemainingRound(StatusProperty.RemainingRound - 1);
            ApplyDamage();
        }
        
        private async void ApplyDamage()
        {
            var damageCalcData = new DotDamageCalcData
            {
                Damage = 70,
                ElementType = E_ElementType.Wind,
                source = Sourcer,
                target = Owner
            };
            
            // 产生特效
            var vfxInfo = ServiceLocator.Get<IPoolManager>().GetData<VFXInfo>();
            var pos = Owner.GameObject.transform.position + Vector3.forward * 0.5f;
            pos = new Vector3(pos.x, 0.5f, pos.z);
            await ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_Dot_Weather, 
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
