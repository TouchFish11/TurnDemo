using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Status;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Status
{
    /// <summary>
    /// 生机III
    /// </summary>
    [StatusTypeId(321)]
    public class RejuvenationStatusIII : Battle.Status.Status
    {
        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            StatusProperty.SetRemainingRound(StatusProperty.RemainingRound - 1);
            owner.TakeHeal(60);
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
