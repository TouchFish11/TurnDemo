using System.Collections;
using Core.DI;
using Core.Pool;
using Core.Serialize.Binary;
using Core.Utility;
using HotUpdate.Base.Component;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Utility;

using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill
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
        
        protected AbyssGiftSkill(IBattleEntityObject caster, int skillId, BinaryDataManager binaryDataManager) : base(caster, skillId, binaryDataManager)
        {
            
        }

        protected override void InitProjectile()
        {
            // 重新初始化投射物数据
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            // 更新投射物变换信息
            projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position, Quaternion.identity);
            vFXInfo = DIContainer.GetInstance<IPoolManager>().GetData<VFXInfo>();
        }
        
        protected override IEnumerator OnCast(IBattleContext context)
        {
            // 技能释放前短暂延迟
            yield return new WaitForSeconds(0.1f);
            // 获取施法者的动画组件
            var animationComponent = Caster.GetComponent<IBattleAnimationComponent>();
            
            // 根据配置表设置技能对应的动画状态
            animationComponent.SetAnimationState(SkillInfo.f_animationType);
            // 动画切换到第一段
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(Attack01));
            yield return UpdateCamera_01();
            
            // 第一段VEX
            CreateVFX_01();
            
            // 等待第一段VFX结束
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
            yield return TaskUtility.WaitForTask(DIContainer.GetInstance<IBattleCameraManager>().CreateCamera(null, pos, rotation, mask));
        }
        
        private async void CreateVFX_01()
        {
            // 创建特效
            await DIContainer.GetInstance<IVFXManager>().CreateVFX(AssetKeys.VFX_AbyssGiftSkillProjectile, projectileTrans, projectileData, vFXInfo);
        }
    }
}
