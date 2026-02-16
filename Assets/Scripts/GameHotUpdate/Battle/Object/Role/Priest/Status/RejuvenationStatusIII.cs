using Core.Config;
using Core.Pool;
using Core.Service;
using Core.Time;
using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Status;
using Game.VFX;
using UnityEngine;

namespace GameHotUpdate.Battle.Object.Role.Priest.Status
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
        
        private void CreateVFX()
        {
            // 产生特效
            var vfxInfo = ServiceLocator.Get<IPoolManager>().GetData<VFXInfo>();
            var pos = Owner.GameObject.transform.position;
            pos = new Vector3(pos.x, 0.5f, pos.z);
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_Heal, 
                null, pos, Quaternion.identity, vfxInfo);

            ServiceLocator.Get<ITimerManager>().CreateTimer(false, 700, () =>
            {
                vfxInfo.IsStop = true;
            });
        }
    }
}
