using System.Collections;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Ultimate
{
    public class WarriorUltimateSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            var caster = SkillContext.Caster;
            var mainTarget = SkillContext.MainTarget;
            
            // 瞬移到目标身前（目标位置向前偏移，避免重叠）
            caster.GameObject.transform.position = mainTarget.GameObject.transform.position - Vector3.forward;
            // 等待0.1秒
            yield return SkillHelper.Delay(100);
            // 切换终结技技能动画
            var animationComponent = caster.GetComponent<BattleAnimationComponent>(); 
            yield return animationComponent.PlayToTarget(AnimNames[0]);
            
            // 初始化终结技核心特效数据（位置上移0.9米，避免穿模）
            SkillContext.ProjectileData = new ProjectileData(caster, mainTarget, SkillContext.AllTargets, SkillContext);
            SkillContext.ProjectileTrans = new ProjectileTrans(caster.GameObject.transform.position + Vector3.up * 0.9f, Quaternion.identity);
            SkillContext.VFXInfo = poolManager.GetData<VFXInfo>();
            
            // 创建终结技核心攻击特效
            var task = vfxManager.CreateVFX(AssetKeys.VFX_WarriorUltimateSkill, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo);
            yield return SkillHelper.WaitForCreateVFX(SkillContext, task);
        }
    }
}
