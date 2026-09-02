using System.Threading.Tasks;
using Core.DI;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Object.StateMeachine
{
    /// <summary>
    /// 角色回合开始逻辑节点
    /// </summary>
    public class RoleTurnStartNode : ITurnStartNode
    {       
        [Inject] private IBattleManager _battleManager;
        
        public async Task Execute(IBattleEntityObject battleObject)
        {
            battleObject.GetComponent<StatusComponent>().SettlementTurnStart();
            await _battleManager.BattleService.PlaySettlementDot(battleObject);
        }
    }
}
