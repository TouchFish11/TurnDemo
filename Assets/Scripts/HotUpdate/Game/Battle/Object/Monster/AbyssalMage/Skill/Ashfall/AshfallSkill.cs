using System.Collections;
using Core.DI;
using Core.Pool;
using Core.Serialize.Binary;
using HotUpdate.Base.Component;
using HotUpdate.Base.Utility;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.Ashfall
{
    /// <summary>
    /// 烬陨
    /// </summary>
    public class AshfallSkill : Battle.Skill.Base.Skill
    {
        /// <summary>
        /// 普攻动画02
        /// </summary>
        public static string Attack02 => "Attack02";

        public AshfallSkill(IBattleEntityObject caster, int skillId, BinaryDataManager binaryDataManager) : base(caster, skillId, binaryDataManager)
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
            
            // 动画切换到第二段
            animationComponent.SetAnimationState(SkillInfo.f_animationType);
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(Attack02));
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
            await DIContainer.GetInstance<IVFXManager>().CreateVFX(AssetKeys.VFX_AshfallSkillProjectile, projectileTrans, projectileData, vFXInfo);
        }
    }
}
