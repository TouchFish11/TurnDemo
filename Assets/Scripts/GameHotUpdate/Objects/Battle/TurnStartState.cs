using Game.Battle.Objects;
using GameHotUpdate.Battle.Event.Turn;

namespace GameHotUpdate.Objects.Battle
{
    public class TurnStartState : TurnState
    {
        public TurnStartState(IBattleEntityObject battleEntity) : base(battleEntity)
        {
            
        }

        public override void Enter()
        {
            // 触发回合开始事件
            BattleEntity.Context.GetEventBus().TriggerEvent(new TurnStartEvent(BattleEntity.Context, BattleEntity));

            switch (BattleEntity)
            {
                // 切换阶段
                case PlayerObject:
                    BattleEntity.ChangeState(EActPhase.Operator);
                    break;
                case MonsterObject:
                    BattleEntity.ChangeState(EActPhase.RestoreToughness);
                    break;
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
