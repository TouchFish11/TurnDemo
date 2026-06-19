using System.Collections;
using Core.Utility;
using HotUpdate.Base.Component;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.Slime.Strategys
{
    public class SlimeSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        /// <summary>
        /// 普攻动画状态名称
        /// 当前仅用于普攻技能的动画判断
        /// </summary>
        public static string Attack => "Attack";

        private const string _layerName = AnimationUtility.Skill_Layer_Name;
        private readonly string _stateName = Attack;
        private const float _targetEndProgress = 0.9f;
        private const string _vfxName = AssetKeys.VFX_MonsterAttackSkill;

        public override IEnumerator Execute()
        {
            // 获取施法者的动画组件
            var animationComponent = SkillContext.Caster.GetComponent<IBattleAnimationComponent>();
            // 根据配置表设置技能对应的动画状态
            animationComponent.SetAnimationState(SkillContext.SkillInfo.f_animationType);
            // 等待动画播放到普攻状态（Attack）
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(_layerName).IsName(_stateName));
            // 等待动画播放至90%且特效已结束，确保技能流程完整
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(_layerName).normalizedTime >= _targetEndProgress && !SkillContext.VFXInfo.IsAlive);
            
            // 创建普攻特效：从资源配置中获取普攻特效资源并生成
            var task = vfxManager.CreateVFX(_vfxName, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo);
            yield return TaskUtility.WaitForTask(task, projectile => SkillContext.Projectile = projectile);
        }
    }
}
