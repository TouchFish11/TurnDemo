using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Summon;

namespace HotUpdate.Game.Battle.Event.Skill
{
    public class SummonCreatedEvent : BattleEvent
    {
        public ISummon Summon { get; }
        
        public IBattleEntityObject Owner { get; }

        public SummonCreatedEvent(IBattleContext context, ISummon summon, IBattleEntityObject owner) : base(context)
        {
            Summon = summon;
            Owner = owner;
        }
    }
}
