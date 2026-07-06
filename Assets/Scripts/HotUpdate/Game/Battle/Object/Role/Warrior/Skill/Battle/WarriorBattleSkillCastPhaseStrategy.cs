using System.Collections;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Battle
{
    public class WarriorBattleSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        // 战斗攻击动画状态名称
        private const string BattleAttackState = "BattleAttack";
        
        public override IEnumerator Execute()
        {
            // 获取施法者的动画组件
            var animationComponent = SkillContext.Caster.GetComponent<BattleAnimationComponent>();
            // 切换到技能配置的动画状态
            animationComponent.SetSkillState(SkillContext.SkillInfo.f_animName);
            // 等待动画切换到战斗攻击状态
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationLayer.Skill_Layer_Name).IsName(BattleAttackState));
            // 创建战技特效
            var task = vfxManager.CreateVFX(AssetKeys.VFX_WarriorBattleSkill, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo);
            yield return SkillHelper.WaitForCreateVFX(SkillContext, task);
        }
    }
}
