using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Game.Battle.Summon;

namespace HotUpdate.Game.Battle.Event
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
