using System.Collections;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Skill.Normal
{
    public class WizardNormalSkillCastEndPhaseStrategy : SkillCastEndPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            var animationComponent = SkillContext.Caster.GetComponent<BattleAnimationComponent>();
            // 等待动画播放至90%且特效已结束，确保技能流程完整结束
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).normalizedTime >= 0.9f && !SkillContext.VFXInfo.IsAlive);
        }
    }
}
