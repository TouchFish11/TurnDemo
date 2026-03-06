using HotUpdate.Battle.Context;
using HotUpdate.Battle.Object;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Event;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Battle.Event.General
{
    /// <summary>
    /// �����仯�¼�
    /// </summary>
    public class EnergyChangedEvent : BattleEvent
    {
        public IBattleEntityObject Target { get; private set; }
        public int CurrentEnergy { get; private set; }
        public int MaxEnergy { get; private set; }

        /// <summary>
        /// ������ֵ��ԭ��ֵ - ����ֵ��
        /// ����Ϊ�������ٸ���Ϊ��������
        /// </summary>
        public int DeltaEnergy { get; private set; }

        public EnergyChangedEvent(IBattleContext context, IBattleEntityObject target, int currentEnergy, int maxEnergy, int deltaEnergy) : base(context)
        {
            Target = target;
            CurrentEnergy = currentEnergy;
            MaxEnergy = maxEnergy;
            DeltaEnergy = deltaEnergy;
        }
    }
}
