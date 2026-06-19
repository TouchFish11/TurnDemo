using System.Collections;

namespace HotUpdate.Game.Battle.Skill.Base.Flow
{
    public interface ISkillPhaseStrategy
    {
        void SetSkill(ISkill skill);
        
        IEnumerator Execute();
    }
}
