using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Event.Turn
{
    /// <summary>
    /// 切换实体回合事件
    /// </summary>
    public class SwitchEntityTurnEvent : BattleEvent
    {
        public IBattleEntityObject CurrentBattleEntityObject { get; }
        
        public SwitchEntityTurnEvent(IBattleContext context, IBattleEntityObject newEntity) : base(context)
        {
            CurrentBattleEntityObject = newEntity;
        }
    }
}
