using System.Threading.Tasks;

namespace HotUpdate.Game.Battle.Object.StateMeachine
{
    /// <summary>
    /// 怪物回合开始逻辑节点
    /// </summary>
    public class MonsterTurnStartNode : ITurnStartNode
    {
        public Task Execute(IBattleEntityObject entity)
        {
            return Task.CompletedTask;
        }
    }
}
