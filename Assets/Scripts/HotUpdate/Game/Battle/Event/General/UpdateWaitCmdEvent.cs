using System.Collections.Generic;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Event.General
{
    /// <summary>
    /// 更新指令等待事件
    /// </summary>
    public class UpdateWaitCmdEvent : BattleEvent
    {
        public List<IBattleEntityObject> BattleEntities { get; }
        
        public UpdateWaitCmdEvent(IBattleContext context, List<IBattleEntityObject> battleEntities) : base(context)
        {
            BattleEntities = battleEntities;
        }
    }
}
