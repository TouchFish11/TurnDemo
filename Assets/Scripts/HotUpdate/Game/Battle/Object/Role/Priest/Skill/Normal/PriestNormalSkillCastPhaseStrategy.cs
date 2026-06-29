using System.Collections;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Skill.Normal
{
    public class PriestNormalSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        // 动画状态名称常量：攻击状态（与Animator中状态名对应）
        private const string AttackState = "NormalAttack";
        private const float _targetEndProgress = 0.2f;

        public override IEnumerator Execute()
        {
            yield return SkillHelper.WaitForAnimationPlayTarget(SkillContext, AnimationUtility.Skill_Layer_Name, AttackState, _targetEndProgress);
            // 创建普攻特效：从资源配置中获取普攻特效资源并生成
            var task = vfxManager.CreateVFX(AssetKeys.VFX_Priest_NormalSkill, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo);
            yield return SkillHelper.WaitForCreateVFX(SkillContext, task);
        }
    }
}
