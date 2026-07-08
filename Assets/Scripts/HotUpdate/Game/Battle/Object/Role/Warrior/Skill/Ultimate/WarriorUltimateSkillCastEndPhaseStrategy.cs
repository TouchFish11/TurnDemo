using System.Collections;
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
            
            // 等待动画播放和特效结束
            yield return BattleAnimationComponent.WaitForPlay(LastAnimationName);
            yield return new WaitUntil(() => !SkillContext.VFXInfo.IsAlive);
            // 重置角色位置到战斗初始点位
            caster.GameObject.transform.position = battleCoordinator.GetRoleTransByIndex(caster.EntityPosIndex);
            // 等待0.1秒
            yield return SkillHelper.Delay(100);
        }
    }
}
