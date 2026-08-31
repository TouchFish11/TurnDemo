using HotUpdate.Game.Battle.Object.Role;

namespace HotUpdate.Game.Battle.Object.StateMeachine
{
    /// <summary>
    /// 回合进行中状态
    /// </summary>
    public class TurnExecutingState : TurnState
    {
        private readonly ITurnActionDriver _driver;
        
        public TurnExecutingState(IBattleEntityObject battleEntity, ITurnActionDriver driver) : base(battleEntity)
        { 
            _driver = driver;
        }

        public override async void Enter()
        {
            await _driver.WaitForOperation();
            // 切换状态
            BattleObject.ChangeState(EActPhase.TurnEnd);
        }

        public override void Exit()
        {
            
        }
    }
}
