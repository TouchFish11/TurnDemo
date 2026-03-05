using System.Collections.Generic;
using HotUpdate.Battle.Context;
using HotUpdate.Battle.Object;

namespace HotUpdate.Battle.Event.General
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
