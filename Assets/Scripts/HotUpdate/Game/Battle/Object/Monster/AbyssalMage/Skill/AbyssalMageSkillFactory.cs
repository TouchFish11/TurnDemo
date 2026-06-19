using System.Collections.Generic;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.Handler;
using HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Strategys;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Handler;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill
{
    /// <summary>
    /// 深渊法师技能工厂
    /// </summary>
    public class AbyssalMageSkillFactory : SkillFactory
    {
        protected override SKillBuildData CreateSKillBuildData(int skillId)
        {
            var strategy = new AbyssalMageProjectileEventProcessStrategy();
            var projectileInitStrategy = new AbyssalMageProjectileInitStrategy();
            ISkillCastPostHandler handler = null;
            List<ISkillNode> effects = null;
            switch (skillId)
            {
                case 103:
                case 104:
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    effects = SkillPhaseBuilder.
                        AddMonsterPreNode().
                        AddTargetSelectNode().
                        AddProjectileInitNode(projectileInitStrategy.InitAttack).
                        AddUpdateCameraNode().
                        AddDelayNode(0.1f).
                        AddPlayAnimationNode(AnimationUtility.Skill_Layer_Name, Attack, 0.9f).
                        AddCreateProjectileNode(AssetKeys.VFX_MonsterAttackSkill).
                        AddProcessProjectileEventNode(strategy.PriestNormalSkillEvent).
                        AddDelayNode(0.1f).
                        Build();
                    break;
                case 105:
                case 106:
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<AbyssalMageSkillCastPostHandler>();
                    effects = SkillPhaseBuilder.
                        AddMonsterPreNode().
                        AddTargetSelectNode().
                        AddProjectileInitNode(projectileInitStrategy.InitAttack).
                        AddUpdateCameraNode().
                        AddDelayNode(0.1f).
                        AddPlayAnimationNode(AnimationUtility.Skill_Layer_Name, Attack, 0.9f).
                        AddCreateProjectileNode(AssetKeys.VFX_MonsterAttackSkill).
                        AddProcessProjectileEventNode(strategy.PriestNormalSkillEvent).
                        AddDelayNode(0.1f).
                        Build();
                    break;
            }
            var sKillBuildData = new SKillBuildData(handler, TODO);
            return sKillBuildData;
        }
    }
}
