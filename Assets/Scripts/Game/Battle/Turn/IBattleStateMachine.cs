using System.Collections;
using System.Threading.Tasks;
using Game.Battle.Command;
using Game.Battle.Context;
using Game.Battle.Objects;

namespace Game.Battle.Turn
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
        /// 更新实体看向
        /// </summary>
        /// <param name="target"></param>
        //void UpdateEntityLookAt(IBattleEntityObject target);

        /// <summary>
        /// 插入命令
        /// </summary>
        /// <param name="skill"></param>

        //void InsertCommand(ICommand command);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="battlePhase"></param>
        void ChangeState(EBattlePhase battlePhase);

        void Dispose();
    }
}
