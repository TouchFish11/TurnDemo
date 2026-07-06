using System.Collections;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Normal
{
    public class WarriorNormalSkillCastEndPhaseStrategy : SkillCastEndPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            var animationComponent = SkillContext.Caster.GetComponent<BattleAnimationComponent>();
            var animator = animationComponent.Animator;
            // 等待动画播放到90%且特效结束
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationLayer.Skill_Layer_Name).normalizedTime >= 0.9f && !SkillContext.VFXInfo.IsAlive);
            // 重置角色本地位置（防止动画位移残留）
            animator.transform.localPosition = Vector3.zero;
            yield return SkillHelper.Delay(100);
        }
    }
}
