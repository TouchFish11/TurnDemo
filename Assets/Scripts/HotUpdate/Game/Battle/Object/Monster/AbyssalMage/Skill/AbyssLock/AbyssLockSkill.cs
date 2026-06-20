using System.Collections;
using Core.DI;
using Core.Pool;
using Core.Serialize.Binary;
using Core.Utility;
using HotUpdate.Base.Component;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.AbyssLock
{
    /// <summary>
    /// 渊禁
    /// </summary>
    public class AbyssLockSkill
    {
        /// <summary>
        /// 普攻动画01
        /// </summary>
        public static string Attack01 => "Attack01";
        
        public AbyssLockSkill(IBattleEntityObject caster, int skillId, BinaryDataManager binaryDataManager) : base(caster, skillId, binaryDataManager)
        {
        }

        
        protected override void InitProjectile()
        {
            // 重新初始化投射物数据
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            var pos = new Vector3(0, 0, -3);
            // 更新投射物变换信息
            projectileTrans = new ProjectileTrans(pos, Quaternion.identity);
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
            
            // 等待VFX结束
            yield return new WaitUntil(() => !vFXInfo.IsAlive);
            
            // 技能结束前短暂延迟
            yield return new WaitForSeconds(0.2f);
        }
        
        private IEnumerator UpdateCamera_01()
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
            // 设置相机位置
            yield return TaskUtility.WaitForTask(battleCoordinator.SetCameraTrans(null, pos, rot, mask));
        }
        
        private async void CreateVFX_01()
        {
            // 创建特效
            await DIContainer.GetInstance<IVFXManager>().CreateVFX(AssetKeys.VFX_AbyssLockSkillProjectile, projectileTrans, projectileData, vFXInfo);
        }
    }
}
