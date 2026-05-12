using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Base.Battle.Object;

namespace HotUpdate.Game.Battle.Event.General
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
