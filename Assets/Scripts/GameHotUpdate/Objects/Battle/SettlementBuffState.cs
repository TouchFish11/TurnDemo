using Game.Battle.Objects;
using GameHotUpdate.Status;

namespace GameHotUpdate.Objects.Battle
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
            BattleEntity.GetComponent<StatusComponent>().UpdateStatus();
            // 判断能否行动
            if (BattleEntity.CanAct)
            {
                BattleEntity.ChangeState(EActPhase.TurnStart);
            }
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {

        }
    }
}
