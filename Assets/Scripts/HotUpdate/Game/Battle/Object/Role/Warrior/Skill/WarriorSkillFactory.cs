using System.Collections.Generic;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Object.Role.Warrior.Strategys;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Handler;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill
{
    /// <summary>
    /// 战士技能工厂
    /// </summary>
    public class WarriorSkillFactory : SkillFactory
    {
        // 翻滚动画状态名称
        private const string RollState = "Roll";
        // 攻击动画状态名称
        private const string AttackState = "Attack";
        
        protected override SKillBuildData CreateSKillBuildData(int skillId)
        {
            var projectileInitStrategy = new WarriorProjectileInitStrategy();
            var projectileEventProcessStrategy = new WarriorProjectileEventProcessStrategy();
            ISkillCastPostHandler handler = null;
            List<ISkillNode> effects = null;
            switch (skillId)
            {
                case 10:    // 普攻
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    effects = SkillPhaseBuilder.
                        AddTargetSelectNode().
                        AddSkillPointCastNode().
                        AddProjectileInitNode(null).
                        AddPlayAnimationNode(AnimationUtility.Skill_Layer_Name, RollState, 0.9f).
                        AddPlayAnimationNode(AnimationUtility.Skill_Layer_Name, AttackState, 0.1f).
                        AddCreateProjectileNode(AssetKeys.VFX_WarriorNormalSkill).
                        AddProcessProjectileEventNode(projectileEventProcessStrategy.NormalSkillEvent).
                        AddDelayNode(0.1f).
                        Build();
                    break;
                case 11:    // 战技
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    effects = SkillPhaseBuilder.
                        AddTargetSelectNode().
                        AddSkillPointCastNode().
                        AddProjectileInitNode(projectileInitStrategy.BattleSkillInit).
                        AddPlayAnimationNode(AnimationUtility.Skill_Layer_Name, AttackState, 0.2f).
                        AddCreateProjectileNode(AssetKeys.VFX_Priest_NormalSkill).
                        AddProcessProjectileEventNode(projectileEventProcessStrategy.BattleSkillEvent).
                        AddDelayNode(0.1f).
                        Build();
                    break;
                case 12:    // 终结技
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    effects = SkillPhaseBuilder.
                        AddUltimateDisplayIllustrationNode().
                        AddUltimatePoseNode(AssetKeys.VFX_WarriorUltimatePose).
                        AddUltimateWaitTriggerNode().
                        AddTargetSelectNode().
                        AddUltimateFlowNode(new WarriorUltimateFlowStrategy()).
                        AddProcessProjectileEventNode(projectileEventProcessStrategy.UltimateSkillEvent).
                        AddDelayNode(0.1f).
                        Build();
                    break;
            }

            var sKillBuildData = new SKillBuildData(handler, TODO);
            return sKillBuildData;
        }
    }
}
