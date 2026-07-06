using System.Collections;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;

namespace HotUpdate.Game.Battle.Object.Monster.Slime.Skill.Normal
{
    public class SlimeSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        private const float _targetEndProgress = 0.5f;

        public override IEnumerator Execute()
        {
            yield return SkillHelper.WaitForAnimationPlayTarget(SkillContext, AnimationLayer.Skill_Layer_Name, SkillContext.SkillInfo.f_animName, _targetEndProgress);
            // 创建普攻特效：从资源配置中获取普攻特效资源并生成
            var task = vfxManager.CreateVFX(AssetKeys.VFX_MonsterAttackSkill, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo);
            yield return SkillHelper.WaitForCreateVFX(SkillContext, task);
        }
    }
}
