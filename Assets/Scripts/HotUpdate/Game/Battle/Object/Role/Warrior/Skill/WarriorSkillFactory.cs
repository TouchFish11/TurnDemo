using System.Collections.Generic;
using Core.DI;
using HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Battle;
using HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Normal;
using HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Ultimate;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.Battle.Skill.Base.Phase;
using HotUpdate.Game.Battle.Skill.Handler;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill
{
    /// <summary>
    /// 战士技能工厂
    /// </summary>
    public class WarriorSkillFactory : SkillFactory
    {
        protected override SKillBuildData CreateSKillBuildData(int skillId)
        {
            ISkillCastPostHandler handler = null;
            List<ISkillFlowPhase> phases = null;
            var flow = new SkillFlow();
            switch (skillId)
            {
                case 10:    // 普攻
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    phases = skillPhaseBuilder.
                        AddSkillPreCastPhase(DIContainer.Create<WarriorNormalSkillPreCastPhaseStrategy>()).
                        AddSkillCastPhase(DIContainer.Create<WarriorNormalSkillCastPhaseStrategy>()).
                        AddSkillEventProcessPhase(DIContainer.Create<WarriorNormalSkillEventProcessPhaseStrategy>()).
                        AddSkillCastEndPhase(DIContainer.Create<WarriorNormalSkillCastEndPhaseStrategy>()).
                        Build();
                    break;
                case 11:    // 战技
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    phases = skillPhaseBuilder.
                        AddSkillPreCastPhase(DIContainer.Create<WarriorBattleSkillPreCastPhaseStrategy>()).
                        AddSkillCastPhase(DIContainer.Create<WarriorBattleSkillCastPhaseStrategy>()).
                        AddSkillEventProcessPhase(DIContainer.Create<WarriorBattleSkillEventProcessPhaseStrategy>()).
                        AddSkillCastEndPhase(DIContainer.Create<WarriorBattleSkillCastEndPhaseStrategy>()).
                        Build();
                    break;
                case 12:    // 终结技
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    phases = skillPhaseBuilder.
                        AddSkillPreCastPhase(DIContainer.Create<WarriorUltimateSkillPreCastPhaseStrategy>()).
                        AddSkillCastPhase(DIContainer.Create<WarriorUltimateSkillCastPhaseStrategy>()).
                        AddSkillEventProcessPhase(DIContainer.Create<WarriorUltimateSkillEventProcessPhaseStrategy>()).
                        AddSkillCastEndPhase(DIContainer.Create<WarriorUltimateSkillCastEndPhaseStrategy>()).
                        Build();
                    break;
            }

            flow.RegisterPhases(phases);
            var sKillBuildData = new SKillBuildData(handler, flow);
            return sKillBuildData;
        }
    }
}
