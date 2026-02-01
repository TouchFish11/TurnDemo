using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Objects;

namespace GameHotUpdate.Battle.Event.General
{
    /// <summary>
    /// ���ܱ仯�¼�
    /// </summary>
    public class ShieldChangedEvent : BattleEvent
    {
        public IBattleEntityObject Target { get; }

        public int CurrentShield { get; }

        /// <summary>
        /// ���ܱ仯ֵ
        /// ԭʼ - ��ֵ������Ϊ���٣�����Ϊ����
        /// </summary>
        public int DeltaShield { get; }

        /// <summary>
        /// ���ܻ�׼ֵ
        /// </summary>
        public int ReferenceShield { get; } = 10000;


        public ShieldChangedEvent(IBattleContext context, int currentShield, IBattleEntityObject target, int deltaShield) : base(context)
        {
            CurrentShield = currentShield;
            Target = target;
            DeltaShield = deltaShield;
        }
    }
}
