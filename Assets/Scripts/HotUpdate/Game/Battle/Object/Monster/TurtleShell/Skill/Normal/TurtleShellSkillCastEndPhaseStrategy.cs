using System.Collections;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.TurtleShell.Skill.Normal
{
    public class TurtleShellSkillCastEndPhaseStrategy : SkillCastEndPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            yield return new WaitWhile(() => SkillContext.VFXInfo.IsAlive);
            yield return SkillHelper.Delay(100);
        }
    }
}
