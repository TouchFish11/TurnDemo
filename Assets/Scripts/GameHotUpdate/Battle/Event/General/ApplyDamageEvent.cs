using Game.Battle.Context;
using Game.Battle.Damage;
using Game.Battle.Event;

namespace GameHotUpdate.Battle.Event.General
{
    /// <summary>
    /// Ӧ���˺��¼�
    /// �˺��������������˺�����������
    /// </summary>
    public class ApplyDamageEvent : BattleEvent
    {
        /// <summary>
        /// �˺����
        /// </summary>
        public DamageResult DamageResult {  get; private set; } 

        public ApplyDamageEvent(IBattleContext context, DamageResult damageResult) : base(context)
        {
            DamageResult = damageResult;
        }
    }
}
