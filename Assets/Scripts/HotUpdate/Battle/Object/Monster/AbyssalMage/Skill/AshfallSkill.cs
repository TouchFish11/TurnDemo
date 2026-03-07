using System.Collections;
using Core.Pool;
using Core.Service;
using HotUpdate.Battle.Skill.Base;
using HotUpdate.Common;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.VFX;
using UnityEngine;

namespace HotUpdate.Battle.Object.Monster.AbyssalMage.Skill
{
    /// <summary>
    /// 烬陨
    /// </summary>
    public class AshfallSkill : MonsterSkill
    {
        /// <summary>
        /// 普攻动画02
        /// </summary>
        public static string Attack02 => "Attack02";

        public AshfallSkill(IBattleEntityObject caster, int skillId) : base(caster, skillId)
        {
        }

        protected override void InitProjectile()
        {
            // 重新初始化投射物数据
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            var pos = new Vector3(0, 0, -3);
            // 更新投射物变换信息
            projectileTrans = new ProjectileTrans(pos, Quaternion.identity);
            vFXInfo = ServiceLocator.Get<IPoolManager>().GetData<VFXInfo>();
        }

        protected override IEnumerator OnCast(IBattleContext context)
        {
            // 技能释放前短暂延迟
            yield return new WaitForSeconds(0.1f);
            // 获取施法者的动画组件
            var animationComponent = Caster.GetComponent<BattleAnimationComponent>();
            
            // 动画切换到第二段
            animationComponent.SetAnimationState((E_AnimationType)SkillInfo.f_animationType);
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(Attack02));
            // 第二段VEX
            CreateVFX_02();
            // 等待第二段VFX结束
            yield return new WaitUntil(() => !vFXInfo.IsAlive);
            
            // 技能结束前短暂延迟
            yield return new WaitForSeconds(0.2f);
        }
        
        private async void CreateVFX_02()
        {
            // 创建特效
            await ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_AshfallSkillProjectile, projectileTrans, projectileData, vFXInfo);
        }
    }
}
