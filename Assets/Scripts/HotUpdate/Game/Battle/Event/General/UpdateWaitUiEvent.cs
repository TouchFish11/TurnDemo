using System.Collections.Generic;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.UI;

namespace HotUpdate.Game.Battle.Event.General
{
    /// <summary>
    /// 更新等待列表中的UI
    /// </summary>
    public class UpdateWaitUiEvent : BattleEvent
    {
        public List<IDisplayPendingExecution> DisplayPendingCommands { get; }
        
        public UpdateWaitUiEvent(IBattleContext context, List<IDisplayPendingExecution> displayPendingCommands) : base(context)
        {
            DisplayPendingCommands = displayPendingCommands;
        }
    }
}
