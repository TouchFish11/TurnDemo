using System.Collections;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Skill.Ultimate
{
    public class WizardUltimateSkillCastEndPhaseStrategy : SkillCastEndPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            yield return BattleAnimationComponent.WaitForPlay(LastAnimationName);
            yield return new WaitUntil(() => !SkillContext.VFXInfo.IsAlive);
        }
    }
}
