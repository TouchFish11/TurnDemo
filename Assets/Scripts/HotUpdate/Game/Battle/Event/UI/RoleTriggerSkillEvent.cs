using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Base.Battle.Object;

namespace HotUpdate.Game.Battle.Event.UI
{
    /// <summary>
	/// 角色触发技能事件
	/// 技能按键按下时触发该事件；非终结技技能使用该事件
    /// </summary>
    public class RoleTriggerSkillEvent : BattleEvent
    {
        public int SkillId { get; private set; }

        public IBattleEntityObject Caster { get; private set; }

        public RoleTriggerSkillEvent(IBattleContext context, int skillId, IBattleEntityObject battleEntity) : base(context)
        {
            SkillId = skillId;
            Caster = battleEntity;
        }
    }
}
