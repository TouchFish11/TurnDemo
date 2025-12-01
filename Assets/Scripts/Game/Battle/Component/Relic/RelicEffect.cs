
namespace GameLogic.BattleMoudule.Relic
{
    /// <summary>
    /// ÒÅÆ÷´ÊÌõÐ§¹û
    /// </summary>
    public struct RelicEffect
    {
        public E_RelicBoun RelicBoun { get; }

        public int BounValue { get; }

        public RelicEffect(E_RelicBoun relicBoun, int bounValue)
        {
            RelicBoun = relicBoun;
            BounValue = bounValue;
        }
    }
}
