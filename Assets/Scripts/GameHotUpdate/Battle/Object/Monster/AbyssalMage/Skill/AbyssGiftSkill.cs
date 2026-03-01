using System.Collections;
using Core.Log;
using Core.Pool;
using Core.Service;
using Core.Utility;
using GameHotUpdate.Animation;
using GameHotUpdate.Animation.Component;
using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Layer;
using GameHotUpdate.Battle.Skill.Base;
using GameHotUpdate.Camera;
using GameHotUpdate.Config;
using GameHotUpdate.VFX;
using UnityEngine;

namespace GameHotUpdate.Battle.Object.Monster.AbyssalMage.Skill
{
    /// <summary>
    /// 深渊之赐
    /// </summary>
    public class AbyssGiftSkill : MonsterSkill
    {
        /// <summary>
        /// 动画01
        /// </summary>
        public static string Attack01 => "Attack01";
        
        public AbyssGiftSkill(IBattleEntityObject caster, int skillId) : base(caster, skillId)
        {
        }

        protected override void InitProjectile()
        {
            // 重新初始化投射物数据
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            // 更新投射物变换信息
            projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position, Quaternion.identity);
            vFXInfo = ServiceLocator.Get<IPoolManager>().GetData<VFXInfo>();
        }
        
        protected override IEnumerator OnCast(IBattleContext context)
        {
            // 技能释放前短暂延迟
            yield return new WaitForSeconds(0.1f);
            // 获取施法者的动画组件
            var animationComponent = Caster.GetComponent<BattleAnimationComponent>();
            
            // 根据配置表设置技能对应的动画状态
            animationComponent.SetAnimationState((E_AnimationType)SkillInfo.f_animationType);
            // 动画切换到第一段
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(Attack01));
            yield return UpdateCamera_01();
            
            // 第一段VEX
            CreateVFX_01();
            
            // 等待第一段VFX结束
            yield return new WaitUntil(() => !vFXInfo.IsAlive);
            LogManager.Log($"-------------------特效结束");
            
            // 技能结束前短暂延迟
            yield return new WaitForSeconds(0.2f);
        }
        
        private IEnumerator UpdateCamera_01()
        {
            // 设置Mask
            var mask = LayerGeter.GetPreBitLayer() | (1 << Caster.GameObject.layer);
            
            // 切换相机视角
            var monsterPos = Caster.GameObject.transform.position;
            monsterPos = new Vector3(monsterPos.x, 1, monsterPos.z);
            var pos = monsterPos + Caster.GameObject.transform.forward * 4;
            var rotation = Quaternion.LookRotation(monsterPos - pos);
            
            // 创建相机
            yield return TaskUtility.WaitForTask(ServiceLocator.Get<IBattleCameraManager>().CreateCamera(null, pos, rotation, mask));
        }
        
        private void CreateVFX_01()
        {
            // 创建特效
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_AbyssGiftSkillProjectile, projectileTrans, projectileData, vFXInfo);
        }
    }
}
