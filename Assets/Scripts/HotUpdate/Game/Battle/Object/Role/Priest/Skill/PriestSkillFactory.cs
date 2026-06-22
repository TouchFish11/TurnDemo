using System.Collections.Generic;
using HotUpdate.Game.Battle.Object.Role.Priest.Skill.Battle;
using HotUpdate.Game.Battle.Object.Role.Priest.Skill.Normal;
using HotUpdate.Game.Battle.Object.Role.Priest.Skill.Ultimate;
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

        protected override SKillBuildData CreateSKillBuildData(int skillId)
        {
            ISkillCastPostHandler handler = null;
            List<ISkillFlowPhase> phases = null;
            var flow = new SkillFlow();
            switch (skillId)
            {
                case 30:    // 普攻
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    phases = skillPhaseBuilder.
                        AddSkillPreCastPhase(new PriestNormalSkillPreCastPhaseStrategy()).
                        AddSkillCastPhase(new PriestNormalSkillCastPhaseStrategy()).
                        AddSkillEventProcessPhase(new PriestNormalSkillEventProcessPhaseStrategy()).
                        AddSkillCastEndPhase(new PriestNormalSkillCastEndPhaseStrategy()).
                        Build();
                    break;
                case 31:    // 战技
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    phases = skillPhaseBuilder.
                        AddSkillPreCastPhase(new PriestBattleSkillPreCastPhaseStrategy()).
                        AddSkillCastPhase(new PriestBattleSkillCastPhaseStrategy()).
                        AddSkillEventProcessPhase(new PriestBattleSkillEventProcessPhaseStrategy()).
                        AddSkillCastEndPhase(new PriestNormalSkillCastEndPhaseStrategy()).
                        Build();
                    break;
                case 32:    // 终结技
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    phases = skillPhaseBuilder.
                        AddSkillPreCastPhase(new PriestUltimateSkillPreCastPhaseStrategy()).
                        AddSkillCastPhase(new PriestUltimateSkillCastPhaseStrategy()).
                        AddSkillEventProcessPhase(new PriestUltimateSkillEventProcessPhaseStrategy()).
                        AddSkillCastEndPhase(new PriestNormalSkillCastEndPhaseStrategy()).
                        Build();
                    break;
            }
            
            flow.RegisterPhases(phases);
            var sKillBuildData = new SKillBuildData(handler, flow);
            return sKillBuildData;
        }
    }
}
