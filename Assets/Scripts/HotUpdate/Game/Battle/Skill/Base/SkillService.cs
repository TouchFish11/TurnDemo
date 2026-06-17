using Core.DI;
using HotUpdate.Game.Battle.TargetSelect;

namespace HotUpdate.Game.Battle.Skill.Base
{
    /// <summary>
    /// 技能服务
    /// </summary>
    public class SkillService
    {
        [Inject] private ITargetSelectManager _targetSelectManager;
        

    }
}
