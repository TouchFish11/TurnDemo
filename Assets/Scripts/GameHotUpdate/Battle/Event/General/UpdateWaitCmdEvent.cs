using System.Collections.Generic;
using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Objects;

namespace GameHotUpdate.Battle.Event.General
{
    public class UpdateWaitCmdEvent : BattleEvent
    {
        public List<IBattleEntityObject> BattleEntities { get; }
        
        public UpdateWaitCmdEvent(IBattleContext context, List<IBattleEntityObject> battleEntities) : base(context)
        {
            BattleEntities = battleEntities;
        }
    }
}
