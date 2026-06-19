using System.Collections.Generic;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Object.Role.Priest.Strategys;
using HotUpdate.Game.Battle.Object.Role.Warrior.Strategys;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.Battle.Skill.Base.Phase;
using HotUpdate.Game.Battle.Skill.Handler;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Skill
{
    /// <summary>
    /// 牧师技能工厂
    /// </summary>
    public class PriestSkillFactory : SkillFactory
    {
        // 动画状态名称常量：攻击状态（与Animator中状态名对应）
        private const string AttackState = "NormalAttack";
        private const string BattleAttackState = "BattleAttack";
        
        protected override SKillBuildData CreateSKillBuildData(int skillId)
        {
            ISkillCastPostHandler handler = null;
            List<ISkillFlowPhase> phases = null;
            var flow = new SkillFlow();
            switch (skillId)
            {
                case 30:    // 普攻
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    phases = SkillPhaseBuilder.
                        AddSkillPreCastPhase(new PriestSkillPreCastPhaseStrategy()).
                        AddSkillCastPhase(new PriestSkillCastPhaseStrategy()).
                        AddSkillEventProcessPhase(new PriestSkillEventProcessPhaseStrategy()).
                        AddSkillCastEndPhase(new PriestSkillCastEndPhaseStrategy()).
                        Build();
                    
                        AddTargetSelectNode().
                        AddSkillPointCastNode().
                        AddProjectileInitNode(projectileInitStrategy.NormalSkillInit).
                        AddPlayAnimationNode(AnimationUtility.Skill_Layer_Name, AttackState, 0.2f).
                        AddCreateProjectileNode(AssetKeys.VFX_Priest_NormalSkill).
                        AddProcessProjectileEventNode(projectileEventProcessStrategy.PriestNormalSkillEvent).
                        AddDelayNode(0.1f).
                        Build();
                    break;
                case 31:    // 战技
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    phases = SkillPhaseBuilder.
                        AddTargetSelectNode().
                        AddSkillPointCastNode().
                        AddProjectileInitNode(projectileInitStrategy.BattleSkillInit).
                        AddPlayAnimationNode(AnimationUtility.Skill_Layer_Name, BattleAttackState, 0.5f).
                        AddCreateProjectileNode(AssetKeys.VFX_Priest_BattleSkill).
                        AddProcessProjectileEventNode(projectileEventProcessStrategy.PriestBattleSkillEvent).
                        Build();
                    break;
                case 32:    // 终结技
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    phases = SkillPhaseBuilder.
                        AddUltimateDisplayIllustrationNode().
                        AddUltimatePoseNode(AssetKeys.VFX_Priest_UltimatePose).
                        AddUltimateWaitTriggerNode().
                        AddTargetSelectNode().
                        AddUltimateFlowNode(new PriestUltimateFlowStrategy()).
                        AddProcessProjectileEventNode(projectileEventProcessStrategy.PriestUltimateSkillEvent).
                        AddDelayNode(0.1f).
                        Build();
                    break;
            }
            
            flow.RegisterPhases(phases);
            var sKillBuildData = new SKillBuildData(handler, flow);
            return sKillBuildData;
        }
    }
}
