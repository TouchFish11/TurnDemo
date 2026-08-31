using System.Threading.Tasks;
using HotUpdate.Game.Battle.Statuses;

namespace HotUpdate.Game.Battle.Object.StateMeachine
{
    /// <summary>
    /// 状态更新和回合开始结算
    /// </summary>
    public class StatusSettlementNode : ITurnStartNode
    {
        public Task Execute(IBattleEntityObject battleObject)
        {
            battleObject.GetComponent<StatusComponent>().UpdateStatus();
            battleObject.GetComponent<StatusComponent>().SettlementTurnStart();
            return Task.CompletedTask;
        }
    }
}
