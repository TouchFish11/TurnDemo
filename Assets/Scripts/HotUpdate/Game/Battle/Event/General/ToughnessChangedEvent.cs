using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Event.General
{
    /// <summary>
    /// 韧性变化事件
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
