using System.Collections;
using Core.DI;
using Core.Pool;
using Core.Utility;
using HotUpdate.Base.Animation;
using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Layer;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Base.Camera;
using HotUpdate.Base.VFX;
using HotUpdate.Common;
using HotUpdate.Game.Battle.Skill.Base;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill
{
    /// <summary>
    /// 渊禁
    /// </summary>
    public class AbyssLockSkill : MonsterSkill
    {
        /// <summary>
        /// 普攻动画01
        /// </summary>
        public static string Attack01 => "Attack01";
        
        public AbyssLockSkill(IBattleEntityObject caster, int skillId) : base(caster, skillId)
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
            var centerPos = DIContainer.GetInstance<IBattleManager>().GetContext().GetProxy().BattlePoint.MonsterCenter.transform
                .position;
            centerPos = new Vector3(3, centerPos.y, centerPos.z);
            DIContainer.GetInstance<IBattleManager>().GetContext().GetProxy().BattlePoint.MonsterCenter.transform.position = centerPos;
            
            // 切换相机视角
            var pos = new Vector3(0, 5, -11.5f);
            var rot = Quaternion.Euler(25, 0, 0);
            yield return TaskUtility.WaitForTask(DIContainer.GetInstance<IBattleCameraManager>().CreateCamera(null, pos, rot, mask));
        }
        
        private async void CreateVFX_01()
        {
            // 创建特效
            await DIContainer.GetInstance<IVFXManager>().CreateVFX(ResKeyCollection.VFX_AbyssLockSkillProjectile, projectileTrans, projectileData, vFXInfo);
        }
    }
}
