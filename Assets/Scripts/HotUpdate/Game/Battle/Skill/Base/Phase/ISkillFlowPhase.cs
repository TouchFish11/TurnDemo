using System.Collections;

namespace HotUpdate.Game.Battle.Skill.Base.Phase
{
    public interface ISkillFlowPhase
    {
        IEnumerator Execute();
    }
}
