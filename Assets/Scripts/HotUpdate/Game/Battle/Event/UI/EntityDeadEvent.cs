using System.Collections.Generic;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Event.UI
{
    /// <summary>
    /// 实体死亡事件，更新行动轴显示，当实体血量为0时触发
    /// </summary>
    public class EntityDeadEvent : BattleEvent
    {
        public List<IBattleEntityObject> DeadEntitys { get; }

        public EntityDeadEvent(IBattleContext context, List<IBattleEntityObject> deadEntitys) : base(context)
        {
            DeadEntitys = deadEntitys;
        }
    }
}
