using Core.Service;
using Core.Singleton;
using HotUpdate.Battle.Skill.Interface;
using HotUpdate.Battle.TargetSelect;

namespace HotUpdate.Battle.Skill.Base
{
    /// <summary>
    /// ���ܹ�����
    /// </summary>
    public class SkillManager : SingletonBase<SkillManager>, ISkillManager
    {
        private SkillManager()
        {

        }

        public void InitSkillTarget(ISkill skill)
        {

            var mainTaget = ServiceLocator.Get<ITargetSelectManager>().GetMainTarget();

            var selectedTargets = ServiceLocator.Get<ITargetSelectManager>().GetTargets();

            skill.Init(mainTaget, selectedTargets);
        }
    }
}
