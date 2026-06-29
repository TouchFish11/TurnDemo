using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Statuses;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Status
{
    /// <summary>
    /// 生机II
    /// </summary>
    [StatusTypeId(311)]
    public class RejuvenationStatusII : StatusBase
    {
        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            base.OnTurnStart(owner, context);
            owner.TakeHeal(50);
            CreateVFX();
        }
        
        private async void CreateVFX()
        {
            // 产生特效
            var vfxInfo = poolManager.GetData<VFXInfo>();
            var pos = Owner.GameObject.transform.position; pos = new Vector3(pos.x, 0.5f, pos.z);
            await vfxManager.CreateVFX(AssetKeys.VFX_Heal, null, pos, Quaternion.identity, vfxInfo);
            timerManager.CreateTimer(false, 700, () =>
            {
                vfxInfo.IsStop = true;
            });
        }
    }
}
