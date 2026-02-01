using Game.Battle.Context;
using Game.Battle.Event;
using Game.Battle.Objects;

namespace GameHotUpdate.Battle.Event.General
{
    /// <summary>
    /// �����¼�
    /// </summary>
    public class ToughnessBrokenEvent : BattleEvent
    {
        /// <summary>
        /// ������
        /// </summary>
        public IBattleEntityObject Breaker { get; }

        /// <summary>
        /// �����͵�Ŀ��
        /// </summary>
        public IBattleEntityObject Target { get; }

        /// <summary>
        /// ��ɻ��Ƶļ�����Ϣ
        /// </summary>
        public SkillInfo SkillInfo { get; }

        public ToughnessBrokenEvent(IBattleContext context, IBattleEntityObject breaker, IBattleEntityObject target, SkillInfo skillInfo) : base(context)
        {
            Breaker = breaker;
            Target = target;
            SkillInfo = skillInfo;
        }
    }
}
