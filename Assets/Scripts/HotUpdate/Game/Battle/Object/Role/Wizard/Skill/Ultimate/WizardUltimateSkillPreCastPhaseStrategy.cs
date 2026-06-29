using System.Collections;
using Core.Utility;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Skill.Ultimate
{
    public class WizardUltimateSkillPreCastPhaseStrategy : SkillPreCastPhaseStrategy
    {
        public override IEnumerator Execute()
        {
            // 显示立绘
            yield return battleCoordinator.ExecutePreUltimateCast(skill.SkillContext.Caster, skill.SkillContext.SkillInfo);
            
            // 终结技动画Pose
            var projectileData = new ProjectileData(SkillContext.Caster, SkillContext.MainTarget, SkillContext.AllTargets, SkillContext);
            var projectileTrans = new ProjectileTrans(SkillContext.Caster.GameObject.transform.position, Quaternion.identity);
            var vFXInfo = poolManager.GetData<VFXInfo>();
            skill.SkillContext.Caster.GetComponent<BattleAnimationComponent>().SetUltimatePose();
            // 终结技Pose特效
            var task = vfxManager.CreateVFX(AssetKeys.VFX_WizardUltimatePose, projectileTrans, projectileData, vFXInfo);
            yield return TaskUtility.WaitForTask(task);
            
            // 等待玩家确认
            yield return SkillHelper.WaitForUltimateConfirm(SkillContext);
            
            // 移除Pose特效
            vfxManager.RemoveVFX(skill.SkillContext.VFXInfo);

            SkillHelper.InitRoleSkillTarget(skill, battleCoordinator);
        }
    }
}
