using System.Collections;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.Ashfall
{
    public class AbyssalMageAshfallSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        /// <summary>
        /// 普攻动画02
        /// </summary>
        public static string Attack02 => "Attack02";
        
        public override IEnumerator Execute()
        {
            // 获取施法者的动画组件
            var animationComponent = SkillContext.Caster.GetComponent<BattleAnimationComponent>();
            
            // 动画切换到第二段
            animationComponent.SetAnimationState(SkillContext.SkillInfo.f_animationType);
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(Attack02));
            // 第二段VEX
            yield return CreateVFX_02();
        }
        
        private IEnumerator CreateVFX_02()
        {
            // 创建特效
            var task = vfxManager.CreateVFX(AssetKeys.VFX_AshfallSkillProjectile, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo);
            yield return SkillHelper.WaitForCreateVFX(SkillContext, task);
        }
    }
}
