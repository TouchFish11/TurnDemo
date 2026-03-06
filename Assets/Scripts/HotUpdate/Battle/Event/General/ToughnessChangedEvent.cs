using HotUpdate.Battle.Context;
using HotUpdate.Battle.Object;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Event;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Battle.Event.General
{
    /// <summary>
    /// ���Ա仯�¼�
    /// </summary>
    public class ToughnessChangedEvent : BattleEvent
    {
        public IBattleEntityObject Target { get; private set; }
        public int CurrentToughness { get; private set; }
        public int MaxToughness { get; private set; }

        public ToughnessChangedEvent(IBattleContext context, IBattleEntityObject battleEntity, int currentToughness, int maxToughness) : base(context)
        {
            Target = battleEntity;
            CurrentToughness = currentToughness;
            MaxToughness = maxToughness;
        }
    }
}
