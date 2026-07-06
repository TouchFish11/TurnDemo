using System.Collections;
using Core.Utility;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.Frostfall
{
    public class AbyssalMageFrostfallSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        /// <summary>
        /// 普攻动画02
        /// </summary>
        public static string Attack02 => "Attack02";
        
        public override IEnumerator Execute()
        {
            // 获取施法者的动画组件
            var animationComponent = SkillContext.Caster.GetComponent<BattleAnimationComponent>();
            // 动画切换到
            animationComponent.SetSkillState(SkillContext.SkillInfo.f_animName);
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationLayer.Skill_Layer_Name).IsName(Attack02));
            // 切换相机视角
            yield return UpdateCamera();
            // VEX
            yield return CreateVFX_02();
        }
        
        private IEnumerator UpdateCamera()
        {
            // 设置Mask
            var mask = LayerGeter.GetPreBitLayer() | LayerGeter.GetRoleBitLayer() | LayerGeter.GetMonsterBitLayer();
            
            // 更新怪物中心点位置
            var centerPos = battleCoordinator.GetMonsterCenterPos();
            centerPos = new Vector3(3, centerPos.y, centerPos.z);
            battleCoordinator.SetMonsterCenterPos(centerPos);
            
            // 切换相机视角
            var pos = new Vector3(0, 5, -11.5f);
            var rot = Quaternion.Euler(25, 0, 0);
            yield return TaskUtility.WaitForTask(battleCoordinator.SetCameraTrans(null, pos, rot, mask));
        }
        
        private IEnumerator CreateVFX_02()
        {
            // 创建特效
            var task = vfxManager.CreateVFX(AssetKeys.VFX_FrostfallSkillProjectile, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo);
            yield return SkillHelper.WaitForCreateVFX(SkillContext, task);
        }
    }
}
