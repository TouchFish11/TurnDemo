
using HotUpdate.Game.Battle.Relic.Relics.Quantum;

namespace HotUpdate.Game.Battle.Relic
{
    /// <summary>
    /// ������װЧ��������������װЧ��������������װ������չ������
    /// </summary>
    public class RelicSetEffectFactory
    {
        public static IRelicSetEffect Create(int setId)
        {
            return setId switch
            {
                0 => new QuantumRelicSetEffect(),
                // ������װʱ����������case��nameof(����װ��) => new ����װЧ����()
                _ => null
            };
        }
    }
}
