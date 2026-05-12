
namespace HotUpdate.Game.Battle.Relic
{
    /// <summary>
    /// ��������Ч��
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
