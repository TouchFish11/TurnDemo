using System.Collections.Generic;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Event;
using HotUpdate.Core.Battle.Object;

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
