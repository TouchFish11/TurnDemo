using System.Collections;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Battle
{
    public class WarriorBattleSkillCastEndPhaseStrategy : SkillCastEndPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            var animationComponent = SkillContext.Caster.GetComponent<BattleAnimationComponent>();
            // 等待动画播放到90%且特效已结束
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).normalizedTime >= 0.9f && !SkillContext.VFXInfo.IsAlive);
            yield return SkillHelper.Delay(100);
        }
    }
}
