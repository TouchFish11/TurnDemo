using System.Collections;
using Core.Config;
using Core.Log;
using Core.Pool;
using Core.Service;
using Core.Utility;
using Game.Animation;
using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Status;
using Game.VFX;
using GameHotUpdate.Animation;
using GameHotUpdate.Battle.Layer;
using GameHotUpdate.Battle.Skill.Base;
using GameHotUpdate.Cameras;
using UnityEngine;

namespace GameHotUpdate.Battle.Object.Monster.AbyssalMage.Skill
{
    /// <summary>
    /// 霜陨
    /// </summary>
    public class FrostfallSkill : MonsterSkill
    {
        /// <summary>
        /// 普攻动画01
        /// </summary>
        public static string Attack01 => "Attack01";
        
        /// <summary>
        /// 普攻动画02
        /// </summary>
        public static string Attack02 => "Attack02";
        
        public FrostfallSkill(IBattleEntityObject caster, int skillId, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, statusAddStrategy)
        {
        }
        
        protected override void InitProjectile()
        {
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
            
            // 动画切换到第二段
            animationComponent.SetAnimationState((E_AnimationType)SkillInfo.f_animationType);
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(Attack02));
            // 切换相机视角
            yield return UpdateCamera_02();
            // 第二段VEX
            CreateVFX_02();
            // 等待第二段VFX结束
            yield return new WaitUntil(() => !vFXInfo.IsAlive);
            
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
        
        private IEnumerator UpdateCamera_02()
        {
            // 设置Mask
            var mask = LayerGeter.GetPreBitLayer() | LayerGeter.GetRoleBitLayer() | LayerGeter.GetMonsterBitLayer();
            
            // 更新怪物中心点位置
            var centerPos = ServiceLocator.Get<IBattleManager>().GetContext().GetProxy().BattlePoint.MonsterCenter.transform
                .position;
            centerPos = new Vector3(3, centerPos.y, centerPos.z);
            ServiceLocator.Get<IBattleManager>().GetContext().GetProxy().BattlePoint.MonsterCenter.transform.position = centerPos;
            
            // 切换相机视角
            var pos = new Vector3(0, 5, -11.5f);
            var rot = Quaternion.Euler(25, 0, 0);
            yield return TaskUtility.WaitForTask(ServiceLocator.Get<IBattleCameraManager>().CreateCamera(null, pos, rot, mask));
        }
        
        private void CreateVFX_01()
        {
            // 初始化投射物数据
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            // 更新投射物变换信息
            projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position, Quaternion.identity);
            vFXInfo = ServiceLocator.Get<IPoolManager>().GetData<VFXInfo>();
            // 创建特效
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_AbyssGiftSkillProjectile, projectileTrans, projectileData, vFXInfo);
        }
        
        private void CreateVFX_02()
        {
            // 重新初始化投射物数据
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            var pos = new Vector3(0, 0, -3);
            // 更新投射物变换信息
            projectileTrans = new ProjectileTrans(pos, Quaternion.identity);
            vFXInfo = ServiceLocator.Get<IPoolManager>().GetData<VFXInfo>();
            // 创建特效
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_FrostfallSkillProjectile, projectileTrans, projectileData, vFXInfo);
        }
    }
}
