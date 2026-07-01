using System.Collections;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Skill.Normal
{
    public class PriestNormalSkillCastEndPhaseStrategy : SkillCastEndPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            // 等待特效结束
            yield return new WaitWhile(() => SkillContext.VFXInfo.IsAlive);
            yield return SkillHelper.Delay(100);
        }
    }
}
