using System.Collections;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Skill.Ultimate
{
    public class WizardUltimateSkillCastEndPhaseStrategy : SkillCastEndPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            // 等待动画播放到90%且特效已结束，确保技能流程完成
            yield return new WaitUntil(() => BattleAnimationComponent.GetCurrentAnimatorStateInfo(AnimationLayer.Skill_Layer_Name).normalizedTime >= 0.9f && !SkillContext.VFXInfo.IsAlive);
        }
    }
}
