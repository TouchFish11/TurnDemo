using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Base.Battle.Object;

namespace HotUpdate.Game.Battle.Event.UI
{
    /// <summary>
    /// ���������¼�
    /// �Ƴ���Ӧ����UI
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
