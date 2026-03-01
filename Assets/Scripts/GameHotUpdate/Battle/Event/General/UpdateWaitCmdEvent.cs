using System.Collections.Generic;
using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Object;

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
