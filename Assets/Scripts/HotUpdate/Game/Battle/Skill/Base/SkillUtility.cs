using Core.DI;
using HotUpdate.Base.Battle.Skill;
using HotUpdate.Base.Battle.TargetSelect;

namespace HotUpdate.Game.Battle.Skill.Base
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
            var mainTaget = DIContainer.GetInstance<ITargetSelectManager>().GetMainTarget();
            var selectedTargets = DIContainer.GetInstance<ITargetSelectManager>().GetTargets();
            skill.Init(mainTaget, selectedTargets);
        }
    }
}
