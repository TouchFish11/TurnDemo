using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Command;
using HotUpdate.Base.Battle.Event;

namespace HotUpdate.Game.Battle.Event.Turn
{
    public class InsertCommandEvent : BattleEvent
    {
        public ICommand Command { get; }
        
        public InsertCommandEvent(IBattleContext context, ICommand command) : base(context)
        {
            Command = command;
        }
    }
}
