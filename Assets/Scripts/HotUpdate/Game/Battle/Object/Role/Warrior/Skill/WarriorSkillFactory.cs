using System.Collections.Generic;
using HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Battle;
using HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Normal;
using HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Ultimate;
using HotUpdate.Game.Battle.Skill;
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
                    phases = SkillPhaseBuilder.
                        AddSkillPreCastPhase(new WarriorNormalSkillPreCastPhaseStrategy()).
                        AddSkillCastPhase(new WarriorNormalSkillCastPhaseStrategy()).
                        AddSkillEventProcessPhase(new WarriorNormalSkillEventProcessPhaseStrategy()).
                        AddSkillCastEndPhase(new WarriorNormalSkillCastEndPhaseStrategy()).
                        Build();
                    break;
                case 11:    // 战技
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    phases = SkillPhaseBuilder.
                        AddSkillPreCastPhase(new WarriorBattleSkillPreCastPhaseStrategy()).
                        AddSkillCastPhase(new WarriorBattleSkillCastPhaseStrategy()).
                        AddSkillEventProcessPhase(new WarriorBattleSkillEventProcessPhaseStrategy()).
                        AddSkillCastEndPhase(new WarriorBattleSkillCastEndPhaseStrategy()).
                        Build();
                    break;
                case 12:    // 终结技
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    phases = SkillPhaseBuilder.
                        AddSkillPreCastPhase(new WarriorUltimateSkillPreCastPhaseStrategy()).
                        AddSkillCastPhase(new WarriorUltimateSkillCastPhaseStrategy()).
                        AddSkillEventProcessPhase(new WarriorUltimateSkillEventProcessPhaseStrategy()).
                        AddSkillCastEndPhase(new WarriorUltimateSkillCastEndPhaseStrategy()).
                        Build();
                    break;
            }

            flow.RegisterPhases(phases);
            var sKillBuildData = new SKillBuildData(handler, flow);
            return sKillBuildData;
        }
    }
}
