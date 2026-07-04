using System.Collections;
using HotUpdate.Game.Battle.Object.Role;

namespace HotUpdate.Game.Battle.Object.StateMeachine
{
    /// <summary>
    /// 回合进行中状态
    /// </summary>
    public class TurnExecutingState : TurnState
    {
        public TurnExecutingState(IBattleEntityObject battleEntity) : base(battleEntity)
        { 

        }

        public override void Enter()
        {
            PlayerObject.StartCoroutine(OnExceuteAction());
        }

        private IEnumerator OnExceuteAction()
        {
            while (PlayerObject.CanAct || PlayerObject.Acting)
            {
                yield return null;
            }
            
            // 切换状态
            PlayerObject.ChangeState(EActPhase.TurnEnd);
        }

        public override void Exit()
        {
            
        }
    }
}
