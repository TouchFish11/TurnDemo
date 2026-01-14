
namespace Game.Battle
{
    /// <summary>
    /// 怪物死亡事件
    /// 移除对应怪物UI
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
