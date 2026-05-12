using System.Collections.Generic;
using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Base.Battle.Object;

namespace HotUpdate.Game.Battle.Event.General
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
