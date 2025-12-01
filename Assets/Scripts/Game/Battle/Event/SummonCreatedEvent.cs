using GameLogic.BattleMoudule.Core;
using GameLogic.BattleMoudule.Summon;

namespace GameLogic.BattleMoudule.Event
{
    /// <summary>
    /// 召唤物创建事件
    /// </summary>
    public class SummonCreatedEvent : BattleEvent
    {
        /// <summary>
        /// 新建的召唤物
        /// </summary>
        public ISummon Summon { get; }

        /// <summary>
        /// 召唤者
        /// </summary>
        public IBattleEntityObject Owner { get; }

        public SummonCreatedEvent(IBattleContext context, ISummon summon, IBattleEntityObject owner) : base(context)
        {
            Summon = summon;
            Owner = owner;
        }


    }
}
