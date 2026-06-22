using System.Collections;
using Core.Utility;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Animation;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Skill.Normal
{
    public class PriestNormalSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        // 动画状态名称常量：攻击状态（与Animator中状态名对应）
        private const string AttackState = "NormalAttack";
        
        private readonly string _layerName = AnimationUtility.Skill_Layer_Name;
        private readonly string _stateName = AttackState;
        private readonly float _targetEndProgress = 0.2f;
        private string _vfxName = AssetKeys.VFX_Priest_NormalSkill;
        
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
