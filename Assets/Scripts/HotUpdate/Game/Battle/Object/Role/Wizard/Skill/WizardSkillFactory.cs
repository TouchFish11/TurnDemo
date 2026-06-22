using System.Collections.Generic;
using HotUpdate.Game.Battle.Object.Role.Wizard.Skill.Battle;
using HotUpdate.Game.Battle.Object.Role.Wizard.Skill.Normal;
using HotUpdate.Game.Battle.Object.Role.Wizard.Skill.Ultimate;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.Battle.Skill.Base.Phase;
using HotUpdate.Game.Battle.Skill.Handler;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Skill
{
    /// <summary>
    /// 法师技能工厂
    /// </summary>
    public class WizardSkillFactory : SkillFactory
    {
        protected override SKillBuildData CreateSKillBuildData(int skillId)
        {
            ISkillCastPostHandler handler = null;
            List<ISkillFlowPhase> phases = null;
            var flow = new SkillFlow();
            
            switch (skillId)
            {
                case 20:    // 普攻
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    phases = skillPhaseBuilder.
                        AddSkillPreCastPhase(new WizardNormalSkillPreCastPhaseStrategy()).
                        AddSkillCastPhase(new WizardNormalSkillCastPhaseStrategy()).
                        AddSkillEventProcessPhase(new WizardNormalSkillEventProcessPhaseStrategy()).
                        AddSkillCastEndPhase(new WizardNormalSkillCastEndPhaseStrategy()).
                        Build();
                    break;
                case 21:    // 战技
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    phases = skillPhaseBuilder.
                        AddSkillPreCastPhase(new WizardBattleSkillPreCastPhaseStrategy()).
                        AddSkillCastPhase(new WizardBattleSkillCastPhaseStrategy()).
                        AddSkillEventProcessPhase(new WizardBattleSkillEventProcessPhaseStrategy()).
                        AddSkillCastEndPhase(new WizardBattleSkillCastEndPhaseStrategy()).
                        Build();
                    break;
                case 22:    // 终结技
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    phases = skillPhaseBuilder.
                        AddSkillPreCastPhase(new WizardUltimateSkillPreCastPhaseStrategy()).
                        AddSkillCastPhase(new WizardUltimateSkillCastPhaseStrategy()).
                        AddSkillEventProcessPhase(new WizardUltimateSkillEventProcessPhaseStrategy()).
                        AddSkillCastEndPhase(new WizardUltimateSkillCastEndPhaseStrategy()).
                        Build();
                    break;
            }
            
            flow.RegisterPhases(phases);
            var sKillBuildData = new SKillBuildData(handler, flow);
            return sKillBuildData;
        }
    }
}
