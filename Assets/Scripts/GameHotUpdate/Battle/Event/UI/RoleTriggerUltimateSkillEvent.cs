using Game.Battle.Context;
using Game.Battle.Objects;

namespace GameHotUpdate.Battle.Event.UI
{
    /// <summary>
    /// 角色终结技触发事件
    /// </summary>
    public class RoleTriggerUltimateSkillEvent : RoleTriggerSkillEvent
    {
        public RoleTriggerUltimateSkillEvent(IBattleContext context, IBattleEntityObject battleEntity, int ultimateSkillId) : base(context, ultimateSkillId, battleEntity)
        {

        }
    }
}
