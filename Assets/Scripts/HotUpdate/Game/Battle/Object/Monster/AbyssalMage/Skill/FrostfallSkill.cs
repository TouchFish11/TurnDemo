using System.Collections;
using Core.DI;
using Core.Pool;
using Core.Utility;
using HotUpdate.Base;
using HotUpdate.Base.Animation;
using HotUpdate.Base.Component;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Utility;
using HotUpdate.Common;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Layer;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill
{
    /// <summary>
    /// 霜陨
    /// </summary>
    public class FrostfallSkill : MonsterSkill
    {
        /// <summary>
        /// 普攻动画02
        /// </summary>
        public static string Attack02 => "Attack02";
        
        public FrostfallSkill(IBattleEntityObject caster, int skillId) : base(caster, skillId)
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
            // 动画切换到
            animationComponent.SetAnimationState(SkillInfo.f_animationType);
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(Attack02));
            // 切换相机视角
            yield return UpdateCamera_02();
            // VEX
            CreateVFX_02();
            // 等待VFX结束
            yield return new WaitUntil(() => !vFXInfo.IsAlive);
            
            // 技能结束前短暂延迟
            yield return new WaitForSeconds(0.2f);
        }
        
        
        private IEnumerator UpdateCamera_02()
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
        
        private async void CreateVFX_02()
        {
            // 创建特效
            await DIContainer.GetInstance<IVFXManager>().CreateVFX(ResKeyCollection.VFX_FrostfallSkillProjectile, projectileTrans, projectileData, vFXInfo);
        }
    }
}
