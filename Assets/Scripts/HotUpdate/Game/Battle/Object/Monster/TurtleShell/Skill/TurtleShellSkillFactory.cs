using System.Collections.Generic;
using Core.DI;
using HotUpdate.Game.Battle.Object.Monster.TurtleShell.Skill.Normal;
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
                    phases = skillPhaseBuilder.
                        AddMonsterCommonPhase().
                        AddSkillPreCastPhase(DIContainer.Create<TurtleShellSkillPreCastPhaseStrategy>()).
                        AddSkillCastPhase(DIContainer.Create<TurtleShellSkillCastPhaseStrategy>()).
                        AddSkillEventProcessPhase(DIContainer.Create<TurtleShellSkillEventProcessPhaseStrategy>()).
                        AddSkillCastEndPhase(DIContainer.Create<TurtleShellSkillCastEndPhaseStrategy>()).
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
