using System.Threading.Tasks;

namespace HotUpdate.Game.Battle.Object.StateMeachine
{
    /// <summary>
    /// 回合状态接口
    /// </summary>
    public interface ITurnState
    {
        IBattleEntityObject BattleObject { get; }
        
        Task Enter();
        
        Task Exit();
    }
}
