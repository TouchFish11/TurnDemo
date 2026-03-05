using HotUpdate.Battle.Context;
using HotUpdate.Battle.Object;
using HotUpdate.Battle.Summon;

namespace HotUpdate.Battle.Event
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
