using System;
using System.Threading.Tasks;
using Core.Log;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Statuses;
using HotUpdate.Game.VFX;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Status
{
    /// <summary>
    /// 生机
    /// </summary>
    [StatusTypeId(301)]
    public class RejuvenationStatus : StatusBase
    {
        protected override async void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            try
            {
                StatusProperty.RemainingRound -= 1;
                StatusProperty.CurrentPine -= 1;
                owner.TakeHeal(20);
                await CreateVFX();
            }
            catch (Exception e)
            {
                Logger.LogException(ELogTags.Battle, e);
            }
        }

        private async Task CreateVFX()
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
