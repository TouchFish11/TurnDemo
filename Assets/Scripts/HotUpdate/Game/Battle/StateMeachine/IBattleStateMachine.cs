using HotUpdate.Game.Battle.Turn;

namespace HotUpdate.Base
{
    /// <summary>
    /// 战斗状态机接口
    /// </summary>
    public interface IBattleStateMachine
    {
        /// <summary>
        /// 战斗循环
        /// </summary>
        /// <returns></returns>
        void StartBattle();
        
        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="battlePhase"></param>
        void ChangeState(EBattlePhase battlePhase);

        void Dispose();
    }
}
