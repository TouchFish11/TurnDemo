using System.Threading.Tasks;
using Core.Service;
using Core.Singleton;
using HotUpdate.Core.Battle.Skill;
using HotUpdate.Core.Battle.TargetSelect;

namespace HotUpdate.Battle.Skill.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class SkillManager : IInitializable, ISkillManager
    {
        public int Priority => -1;

        private SkillManager()
        {

        }
        
        public Task InitAsync()
        {
            return Task.CompletedTask;   
        }

        public void InitSkillTarget(ISkill skill)
        {
            var mainTaget = ServiceLocator.Get<ITargetSelectManager>().GetMainTarget();
            var selectedTargets = ServiceLocator.Get<ITargetSelectManager>().GetTargets();
            skill.Init(mainTaget, selectedTargets);
        }

    }
}
