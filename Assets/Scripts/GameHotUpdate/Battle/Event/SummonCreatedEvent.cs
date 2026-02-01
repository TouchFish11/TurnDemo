using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Objects;
using GameHotUpdate.Battle.Summon;

namespace GameHotUpdate.Battle.Event
{
    /// <summary>
    /// �ٻ��ﴴ���¼�
    /// </summary>
    public class SummonCreatedEvent : BattleEvent
    {
        /// <summary>
        /// �½����ٻ���
        /// </summary>
        public ISummon Summon { get; }

        /// <summary>
        /// �ٻ���
        /// </summary>
        public IBattleEntityObject Owner { get; }

        public SummonCreatedEvent(IBattleContext context, ISummon summon, IBattleEntityObject owner) : base(context)
        {
            Summon = summon;
            Owner = owner;
        }


    }
}
