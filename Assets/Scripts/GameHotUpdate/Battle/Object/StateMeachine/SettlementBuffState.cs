using Game.Battle.Objects;
using GameHotUpdate.Battle.Status;

namespace GameHotUpdate.Battle.Object.StateMeachine
{
    /// <summary>
    /// 结算Buff状态
    /// </summary>
    public class SettlementBuffState : TurnState
    {
        public SettlementBuffState(IBattleEntityObject battleEntity) : base(battleEntity)
        {
            
        }

        public override void Enter()
        {
            // 调用状态组件更新状态
            PlayerObject.GetComponent<StatusComponent>().UpdateStatus();
            // 判断能否行动
            if (PlayerObject.CanAct)
            {
                PlayerObject.ChangeState(EActPhase.TurnStart);
            }
        }

        public override void Exit()
        {

        }
    }
}
