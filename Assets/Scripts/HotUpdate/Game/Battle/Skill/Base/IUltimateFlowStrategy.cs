using System.Collections;

namespace HotUpdate.Game.Battle.Skill.Base
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUltimateFlowStrategy
    {
        IEnumerator ExecuteFlow(SkillContext skillContext);
    }
}
