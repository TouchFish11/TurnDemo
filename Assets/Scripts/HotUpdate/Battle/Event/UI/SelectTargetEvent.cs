using System.Collections.Generic;
using HotUpdate.Battle.Context;
using HotUpdate.Battle.Object;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Event;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Battle.Event.UI
{
    /// <summary>
    /// 选择目标事件
    /// </summary>
    public class SelectTargetEvent : BattleEvent
    {
        /// <summary>
        /// 选择者
        /// </summary>
        public IBattleEntityObject Selecter { get; }
        
        /// <summary>
        /// 选择的所有目标，包含主目标
        /// </summary>
        public List<IBattleEntityObject> SelectedTargets { get; private set; }
        
        /// <summary>
        /// 主目标
        /// </summary>
        public IBattleEntityObject MainTarget { get; private set; }

        public SelectTargetEvent(IBattleContext context, IBattleEntityObject selecter, IBattleEntityObject mainTarget, List<IBattleEntityObject> selectedTargets) : base(context)
        {
            Selecter = selecter;
            MainTarget = mainTarget;
            SelectedTargets = selectedTargets;
        }
    }
}
