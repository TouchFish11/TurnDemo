using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Base.Battle.Object;

namespace HotUpdate.Game.Battle.Event.General
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
