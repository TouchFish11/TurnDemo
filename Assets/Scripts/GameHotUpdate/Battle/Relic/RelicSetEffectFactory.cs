
using GameHotUpdate.Battle.Relic.Relics.Quantum;

namespace GameHotUpdate.Battle.Relic
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
