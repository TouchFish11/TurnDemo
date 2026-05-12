using System.Collections.Generic;
using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Base.Battle.Object;

namespace HotUpdate.Game.Battle.Event.UI
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
