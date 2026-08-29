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
            RoleObject.StartCoroutine(OnExceuteAction());
        }

        private IEnumerator OnExceuteAction()
        {
            while (RoleObject.CanAct || RoleObject.Acting)
            {
                yield return null;
            }
            
            // 切换状态
            RoleObject.ChangeState(EActPhase.TurnEnd);
        }

        public override void Exit()
        {
            
        }
    }
}
