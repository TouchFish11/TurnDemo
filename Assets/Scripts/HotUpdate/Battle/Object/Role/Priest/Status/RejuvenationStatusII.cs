using Core.Pool;
using Core.Service;
using Core.Time;
using HotUpdate.Battle.Context;
using HotUpdate.Battle.Status;
using HotUpdate.Config;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.VFX;
using UnityEngine;

namespace HotUpdate.Battle.Object.Role.Priest.Status
{
    /// <summary>
    /// 生机II
    /// </summary>
    [StatusTypeId(311)]
    public class RejuvenationStatusII : Battle.Status.Status
    {
        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            StatusProperty.SetRemainingRound(StatusProperty.RemainingRound - 1);
            owner.TakeHeal(50);
            CreateVFX();
        }
        
        private async void CreateVFX()
        {
            // 产生特效
            var vfxInfo = ServiceLocator.Get<IPoolManager>().GetData<VFXInfo>();
            var pos = Owner.GameObject.transform.position;
            pos = new Vector3(pos.x, 0.5f, pos.z);
            await ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_Heal, 
                null, pos, Quaternion.identity, vfxInfo);

            ServiceLocator.Get<ITimerManager>().CreateTimer(false, 700, () =>
            {
                vfxInfo.IsStop = true;
            });
        }
    }
}
