using System.Collections;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Skill.Battle
{
    public class WizardBattleSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        // 战斗攻击动画状态名（与Animator中状态名对应）
        private const string BattleAttackState = "BattleAttack";

        public override IEnumerator Execute()
        {
            // 获取释放者的动画组件，用于播放技能动画
            var animationComponent = SkillContext.Caster.GetComponent<BattleAnimationComponent>();
            // 设置动画状态（从技能配置中读取动画类型）
            animationComponent.SetSkillState(SkillContext.SkillInfo.f_animName);
            
            // 等待动画播放到"战斗攻击"状态（确保动画执行到攻击帧再触发后续逻辑）
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationLayer.Skill_Layer_Name).IsName(BattleAttackState));
            
            // 触发技能特效：通过特效管理器创建战技特效
            var task = vfxManager.CreateVFX(AssetKeys.VFX_WizardBattleSkill, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo);
            yield return SkillHelper.WaitForCreateVFX(SkillContext, task);
        }
    }
}
