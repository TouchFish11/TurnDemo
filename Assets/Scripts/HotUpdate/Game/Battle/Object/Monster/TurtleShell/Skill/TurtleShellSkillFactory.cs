using System.Collections.Generic;
using HotUpdate.Game.Battle.Object.Monster.TurtleShell.Strategys;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.Battle.Skill.Base.Phase;
using HotUpdate.Game.Battle.Skill.Handler;

namespace HotUpdate.Game.Battle.Object.Monster.TurtleShell.Skill
{
    /// <summary>
    /// TurtleShell技能工厂
    /// </summary>
    public class TurtleShellSkillFactory : SkillFactory
    {
        protected override SKillBuildData CreateSKillBuildData(int skillId)
        {
            ISkillCastPostHandler handler = null;
            List<ISkillFlowPhase> phases = null;
            var flow = new SkillFlow();
            
            switch (skillId)
            {
                case 102:
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    phases = SkillPhaseBuilder.
                        AddMonsterCommonPhase().
                        AddSkillPreCastPhase(new TurtleShellSkillPreCastPhaseStrategy()).
                        AddSkillCastPhase(new TurtleShellSkillCastPhaseStrategy()).
                        AddSkillEventProcessPhase(new TurtleShellSkillEventProcessPhaseStrategy()).
                        AddSkillCastEndPhase(new TurtleShellSkillCastEndPhaseStrategy()).
                        Build();
                    break;
            }
            
            // 注册阶段
            flow.RegisterPhases(phases);
            var sKillBuildData = new SKillBuildData(handler, flow);
            return sKillBuildData;
        }
    }
}
