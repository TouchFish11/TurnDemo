using System.Collections.Generic;
using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Object;

namespace GameHotUpdate.Battle.Event.UI
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
