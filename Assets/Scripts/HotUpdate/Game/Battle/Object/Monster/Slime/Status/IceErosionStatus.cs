using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Event.General;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Property;
using HotUpdate.Game.Battle.Statuses;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.Slime.Status
{
    /// <summary>
    /// 冰蚀
    /// </summary>
    [StatusTypeId(1011)]
    public class IceErosionStatus : StatusBase, IDotStatus
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
                Damage = 20,
                ElementType = E_ElementType.Ice,
                source = Sourcer,
                target = Owner
            };
            
            // 产生特效
            var vfxInfo = poolManager.GetData<VFXInfo>();
            var pos = Owner.GameObject.transform.position + Vector3.forward * 0.5f;
            pos = new Vector3(pos.x, 0.5f, pos.z);
            await vfxManager.CreateVFX(AssetKeys.VFX_Dot_IceErosion, null, pos, Quaternion.identity, vfxInfo);

            timerManager.CreateTimer(false, 500, () =>
            {
                vfxInfo.IsStop = true;
                // 更新累计伤害UI
                Owner.Context.EventBus.TriggerEvent(new ClearCumulativeDamageEvent(Owner.Context));
            });
            
            Owner.Context.EventBus.TriggerEvent(new CalcDotDamageEvent(Owner.Context, damageCalcData));
        }
    }
}
