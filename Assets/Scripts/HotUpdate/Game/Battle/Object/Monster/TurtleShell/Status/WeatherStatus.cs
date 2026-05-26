using Core.DI;
using Core.Pool;
using Core.Time;
using HotUpdate.Base;
using HotUpdate.Common.Generated;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Damage.Data;
using HotUpdate.Game.Battle.Event.General;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Status;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.TurtleShell.Status
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
            var vfxInfo = DIContainer.GetInstance<IPoolManager>().GetData<VFXInfo>();
            var pos = Owner.GameObject.transform.position + Vector3.forward * 0.5f;
            pos = new Vector3(pos.x, 0.5f, pos.z);
            await DIContainer.GetInstance<IVFXManager>().CreateVFX(AssetKeys.VFX_Dot_Weather, 
                null, pos, Quaternion.identity, vfxInfo);

            DIContainer.GetInstance<ITimerManager>().CreateTimer(false, 500, () =>
            {
                vfxInfo.IsStop = true;
                // 更新累计伤害UI
                Owner.Context.GetEventBus().TriggerEvent(new ClearCumulativeDamageEvent(Owner.Context));
            });
            
            Owner.Context.GetEventBus().TriggerEvent(new CalcDotDamageEvent(Owner.Context, damageCalcData));
        }
    }
}
