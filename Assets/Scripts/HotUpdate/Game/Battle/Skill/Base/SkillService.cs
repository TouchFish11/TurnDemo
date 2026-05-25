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
        
        /// <summary>
        /// 初始化技能目标
        /// </summary>
        /// <param name="skill"></param>
        public void InitSkillTarget(ISkill skill)
        {
            var mainTaget = _targetSelectManager.GetMainTarget();
            var selectedTargets = _targetSelectManager.GetTargets();
            skill.Init(mainTaget, selectedTargets);
        }
    }
}
