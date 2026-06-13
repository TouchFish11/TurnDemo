using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Event.UI
{
    /// <summary>
    /// 怪物死亡事件
    /// 隐藏怪物UI
    /// </summary>
    public class MonsterDeadEvent : BattleEvent
    {
        public IBattleEntityObject DeadMonster { get; }

        public MonsterDeadEvent(IBattleContext context, IBattleEntityObject deadMonster) : base(context)
        {
            DeadMonster = deadMonster;
        }
    }
}
