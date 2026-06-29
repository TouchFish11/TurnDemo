using System.Collections;
using Core.Utility;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.AbyssGift
{
    public class AbyssalMageAbyssGiftSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        /// <summary>
        /// 动画01
        /// </summary>
        public static string Attack01 => "Attack01";
        
        public override IEnumerator Execute()
        {
            // 获取施法者的动画组件
            var animationComponent = SkillContext.Caster.GetComponent<BattleAnimationComponent>();
            // 根据配置表设置技能对应的动画状态
            animationComponent.SetAnimationState(SkillContext.SkillInfo.f_animationType);
            // 动画切换到第一段
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(Attack01));
            
            yield return UpdateCamera_01();
            // 第一段VEX
            yield return CreateVFX_01();
        }
        
        private IEnumerator UpdateCamera_01()
        {
            // 设置Mask
            var mask = LayerGeter.GetPreBitLayer() | (1 << SkillContext.Caster.GameObject.layer);
            // 切换相机视角
            var monsterPos = SkillContext.Caster.GameObject.transform.position;
            monsterPos = new Vector3(monsterPos.x, 1, monsterPos.z);
            var pos = monsterPos + SkillContext.Caster.GameObject.transform.forward * 4;
            var rotation = Quaternion.LookRotation(monsterPos - pos);
            // 创建相机
            yield return TaskUtility.WaitForTask(battleCoordinator.SetCameraTrans(null, pos, rotation, mask));
        }
        
        private IEnumerator CreateVFX_01()
        {
            // 创建特效
            var task = vfxManager.CreateVFX(AssetKeys.VFX_AbyssGiftSkillProjectile, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo);
            yield return SkillHelper.WaitForCreateVFX(SkillContext, task);
        }
    }
}
