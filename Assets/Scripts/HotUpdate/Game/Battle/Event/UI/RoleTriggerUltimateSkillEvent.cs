using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Object;

namespace HotUpdate.Game.Battle.Event.UI
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
