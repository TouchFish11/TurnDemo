using System.Collections;
using Core.Utility;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Object.Monster.Slime.Strategys
{
    public class SlimeUpdateCameraStrategy : UpdateCameraStrategy
    {
        public override IEnumerator UpdateCamera(SkillContext skillContext)
        {
            yield return TaskUtility.WaitForTask(battleCoordinator.UpdateCamera((PlayerObject)SkillContext.MainTarget));
        }
    }
}
