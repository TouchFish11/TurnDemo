using System.Collections;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Skill.Battle
{
    public class WizardBattleSkillCastEndPhaseStrategy : SkillCastEndPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            // 等待动画播放至90%且特效已结束，确保技能完整执行后再结束协程
            yield return new WaitUntil(() => BattleAnimationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).normalizedTime >= 0.9f && !SkillContext.VFXInfo.IsAlive);
        }
    }
}
