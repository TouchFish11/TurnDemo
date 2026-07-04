using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Event.General;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Property;
using HotUpdate.Game.Battle.Statuses;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Status
{
    /// <summary>
    /// 灼烧
    /// </summary>
    [StatusTypeId(1041)]
    public class BurnStatus : StatusBase, IDotStatus
    {
        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            base.OnTurnStart(owner, context);
            ApplyDamage();
        }
        
        private async void ApplyDamage()
        {
            var damageCalcData = new DotDamageCalcData
            {
                Damage = 80,
                ElementType = E_ElementType.Wind,
                source = Sourcer,
                target = Owner
            };
            
            // 产生特效
            var vfxInfo = poolManager.GetData<VFXInfo>();
            var pos = Owner.GameObject.transform.position + Vector3.forward * 0.5f;
            pos = new Vector3(pos.x, 0.5f, pos.z);
            await vfxManager.CreateVFX(AssetKeys.VFX_Dot_Burn, null, pos, Quaternion.identity, vfxInfo);

            timerManager.CreateTimer(false, 500, () =>
            {
                vfxInfo.IsStop = true;
                // 更新累计伤害UI
                Owner.Context.EventBus.TriggerEvent(new ClearCumulativeDamageEvent(Context));
            });
            
            Context.EventBus.TriggerEvent(new CalcDotDamageEvent(Context, damageCalcData));
        }
        
    }
}
