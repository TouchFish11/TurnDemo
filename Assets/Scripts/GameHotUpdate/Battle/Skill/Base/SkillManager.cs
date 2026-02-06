using Core.Service;
using Core.Singleton;
using Game.Battle.Skill;
using Game.Battle.Skill.Interface;
using Game.Battle.TargetSelect;

namespace GameHotUpdate.Battle.Skill.Base
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
