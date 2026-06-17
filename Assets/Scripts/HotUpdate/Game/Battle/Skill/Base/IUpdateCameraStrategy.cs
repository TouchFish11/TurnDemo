using System.Collections;

namespace HotUpdate.Game.Battle.Skill.Base
{
    public interface IUpdateCameraStrategy
    {
        IEnumerator UpdateCamera(SkillContext skillContext);
    }
}
