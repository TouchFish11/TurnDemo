using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Skill.Handler;
using Game.Battle.Status;
using GameHotUpdate.Battle.Event.UI;

namespace GameHotUpdate.Battle.Skill.Skills
{
    /// <summary>
    /// ��Ҽ���
    /// ��ҽ�ɫ���ܼ̳�
    /// </summary>
    public abstract class PlayerSkill : Skill
    {
        public PlayerSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
        {

        }

        /// <summary>
        /// ��Ҽ����ͷ�ǰִ��
        /// ����ս���㡢����UI����߼�
        /// </summary>
        /// <param name="context"></param>
        protected override void OnPreCast(IBattleContext context)
        {
            base.OnPreCast(context);
            // ����ս����
            context.ConsumeSkillPoint(SkillInfo.f_costBP);
            // ����UI���������UI
            context.GetEventBus().TriggerEvent(new PlayerReleaseSkillEvent(context));
        }
    }
}
