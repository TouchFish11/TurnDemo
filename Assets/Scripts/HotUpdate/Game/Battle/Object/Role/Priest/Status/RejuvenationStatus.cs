using Core.DI;
using Core.Pool;
using Core.Time;
using HotUpdate.Base;
using HotUpdate.Common;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Status;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Status
{
    /// <summary>
    /// 生机
    /// </summary>
    [StatusTypeId(301)]
    public class RejuvenationStatus : Battle.Status.Status
    {
        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            StatusProperty.SetRemainingRound(StatusProperty.RemainingRound - 1);
            owner.TakeHeal(20);
            // 创建VFX
            CreateVFX();
        }

        private async void CreateVFX()
        {
            // 产生特效
            var vfxInfo = DIContainer.GetInstance<IPoolManager>().GetData<VFXInfo>();
            var pos = Owner.GameObject.transform.position;
            pos = new Vector3(pos.x, 0.5f, pos.z);
            await DIContainer.GetInstance<IVFXManager>().CreateVFX(ResKeyCollection.VFX_Heal, 
                null, pos, Quaternion.identity, vfxInfo);

            DIContainer.GetInstance<ITimerManager>().CreateTimer(false, 700, () =>
            {
                vfxInfo.IsStop = true;
            });
        }
    }
}
