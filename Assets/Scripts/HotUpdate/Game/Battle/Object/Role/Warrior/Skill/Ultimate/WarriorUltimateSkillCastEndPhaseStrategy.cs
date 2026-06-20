using System.Collections;
using HotUpdate.Base.Component;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Ultimate
{
    public class WarriorUltimateSkillCastEndPhaseStrategy : SkillCastEndPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            var caster = SkillContext.Caster;
            var animationComponent = caster.GetComponent<IBattleAnimationComponent>();
            
            // 等待动画播放到90%（确保特效播放完成）
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).normalizedTime >= 0.9f);
            
            // 重置角色位置到战斗初始点位
            caster.GameObject.transform.position = battleCoordinator.GetRoleTransByIndex(caster.EntityPosIndex);
            
            // 等待0.1秒（位移后缓冲）
            yield return SkillHelper.Delay(100);
        }
    }
}
