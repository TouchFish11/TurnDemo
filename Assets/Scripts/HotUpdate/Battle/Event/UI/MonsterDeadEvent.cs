using HotUpdate.Battle.Context;
using HotUpdate.Battle.Object;

namespace HotUpdate.Battle.Event.UI
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
