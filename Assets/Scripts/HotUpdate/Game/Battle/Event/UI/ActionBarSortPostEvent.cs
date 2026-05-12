using System.Collections.Generic;
using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Base.Battle.Object;

namespace HotUpdate.Game.Battle.Event.UI
{
    /// <summary>
    /// 行动轴排序后事件
    /// </summary>
    public class ActionBarSortPostEvent : BattleEvent
    {
        public IEnumerable<IBattleEntityObject> battleEntities { get; }

        public ActionBarSortPostEvent(IBattleContext context, IEnumerable<IBattleEntityObject> battleEntities) : base(context)
        {
            this.battleEntities = battleEntities;
        }
    }
}
