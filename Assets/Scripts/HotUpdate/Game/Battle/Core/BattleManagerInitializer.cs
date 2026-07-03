using Core.DI;
using HotUpdate.Base.Manager;
using HotUpdate.Game.Battle.Damage;
using HotUpdate.Game.Battle.Event;
using HotUpdate.Game.Battle.Inputs;
using HotUpdate.Game.Battle.TargetSelect;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗管理器初始化器
    /// </summary>
    public class BattleManagerInitializer
    {
        private BattleManagerInitializer
        (
            ITargetSelectManager targetSelectManager,
            IDamageCalcManager damageCalcManager,
            IBattleInputHandler battleInputHandler,
            IBattleEventScheduler battleEventScheduler,
            IBattleCameraManager battleCameraManager,
            IBattleManager battleManager,
            IBattleCoordinator battleCoordinator,
            IBattlePointProxy battlePointProxy
            )
        {
            
        }
    }
}
