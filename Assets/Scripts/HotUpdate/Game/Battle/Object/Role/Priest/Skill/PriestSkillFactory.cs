using System.Collections.Generic;
using Core.DI;
using HotUpdate.Game.Battle.Object.Role.Priest.Skill.Battle;
using HotUpdate.Game.Battle.Object.Role.Priest.Skill.Normal;
using HotUpdate.Game.Battle.Object.Role.Priest.Skill.Ultimate;
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
                        AddSkillPreCastPhase(DIContainer.Create<PriestNormalSkillPreCastPhaseStrategy>()).
                        AddSkillCastPhase(DIContainer.Create<PriestNormalSkillCastPhaseStrategy>()).
                        AddSkillEventProcessPhase(DIContainer.Create<PriestNormalSkillEventProcessPhaseStrategy>()).
                        AddSkillCastEndPhase(DIContainer.Create<PriestNormalSkillCastEndPhaseStrategy>()).
                        Build();
                    break;
                case 31:    // 战技
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    phases = skillPhaseBuilder.
                        AddSkillPreCastPhase(DIContainer.Create<PriestBattleSkillPreCastPhaseStrategy>()).
                        AddSkillCastPhase(DIContainer.Create<PriestBattleSkillCastPhaseStrategy>()).
                        AddSkillEventProcessPhase(DIContainer.Create<PriestBattleSkillEventProcessPhaseStrategy>()).
                        AddSkillCastEndPhase(DIContainer.Create<PriestNormalSkillCastEndPhaseStrategy>()).
                        Build();
                    break;
                case 32:    // 终结技
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    phases = skillPhaseBuilder.
                        AddSkillPreCastPhase(DIContainer.Create<PriestUltimateSkillPreCastPhaseStrategy>()).
                        AddSkillCastPhase(DIContainer.Create<PriestUltimateSkillCastPhaseStrategy>()).
                        AddSkillEventProcessPhase(DIContainer.Create<PriestUltimateSkillEventProcessPhaseStrategy>()).
                        AddSkillCastEndPhase(DIContainer.Create<PriestNormalSkillCastEndPhaseStrategy>()).
                        Build();
                    break;
            }
            
            flow.RegisterPhases(phases);
            var sKillBuildData = new SKillBuildData(handler, flow);
            return sKillBuildData;
        }
    }
}
