using Core.Service;
using HotUpdate.Core.Battle.Skill;
using HotUpdate.Core.Battle.TargetSelect;

namespace HotUpdate.Battle.Skill.Base
{
    /// <summary>
    /// 技能工具类
    /// </summary>
    public static class SkillUtility
    {
        /// <summary>
        /// 初始化技能目标
        /// </summary>
        /// <param name="skill"></param>
        public static void InitSkillTarget(ISkill skill)
        {
            var mainTaget = ServiceLocator.Get<ITargetSelectManager>().GetMainTarget();
            var selectedTargets = ServiceLocator.Get<ITargetSelectManager>().GetTargets();
            skill.Init(mainTaget, selectedTargets);
        }
    }
}
