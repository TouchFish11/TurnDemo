using System.Threading.Tasks;

namespace HotUpdate.Game.Battle.Object.StateMeachine
{
    /// <summary>
    /// 实体状态机回合开始阶段的逻辑节点
    /// </summary>
    public interface ITurnStartNode
    {
        Task Execute(IBattleEntityObject entity);
    }
}
