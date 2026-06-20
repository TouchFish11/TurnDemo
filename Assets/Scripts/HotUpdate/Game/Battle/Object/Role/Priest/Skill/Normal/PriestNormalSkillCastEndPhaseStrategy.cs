using System.Collections;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Skill.Normal
{
    public class PriestNormalSkillCastEndPhaseStrategy : SkillCastEndPhaseStrategy
    {
        private static readonly WaitForSeconds s_waitForSeconds = new(0.1f);
        
        public override IEnumerator Execute()
        {
            yield return s_waitForSeconds;
        }
    }
}
