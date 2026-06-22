using System.Collections;
using Core.Utility;
using HotUpdate.Game.Battle.Animation;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Skill.Ultimate
{
    public class PriestUltimateSkillPreCastPhaseStrategy : SkillPreCastPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            // 都显示立绘
            yield return battleCoordinator.ExecutePreUltimateCast(skill.SkillContext.Caster, skill.SkillContext.SkillInfo);
            
            // 终结技动画Pose
            var projectileData = new ProjectileData(SkillContext.Caster, SkillContext.MainTarget, SkillContext.AllTargets, SkillContext);
            var projectileTrans = new ProjectileTrans(SkillContext.Caster.GameObject.transform.position, Quaternion.identity);
            var vFXInfo = poolManager.GetData<VFXInfo>();
            skill.SkillContext.Caster.GetComponent<IBattleAnimationComponent>().SetUltimatePose();
            // 终结技Pose特效
            var task = vfxManager.CreateVFX(AssetKeys.VFX_Priest_UltimatePose, projectileTrans, projectileData, vFXInfo);
            yield return TaskUtility.WaitForTask(task);

            yield return SkillHelper.WaitForUltimateConfirm(SkillContext);
            
            // 移除Pose特效
            vfxManager.RemoveVFX(skill.SkillContext.VFXInfo);
            // 根据技能配置和选择策略，筛选出技能作用的目标
            SkillHelper.InitSkillTarget(skill, battleCoordinator);
        }
    }
}
