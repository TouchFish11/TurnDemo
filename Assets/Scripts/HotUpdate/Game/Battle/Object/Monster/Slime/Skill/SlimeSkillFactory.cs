using System.Collections.Generic;
using HotUpdate.Game.Battle.Object.Monster.Slime.Strategys;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.Battle.Skill.Base.Phase;
using HotUpdate.Game.Battle.Skill.Handler;

namespace HotUpdate.Game.Battle.Object.Monster.Slime.Skill
{
    /// <summary>
    /// Slime技能工厂
    /// </summary>
    public class SlimeSkillFactory : SkillFactory
    {
        protected override SKillBuildData CreateSKillBuildData(int skillId)
        {
            ISkillCastPostHandler handler = null;
            List<ISkillFlowPhase> phases = null;
            var flow = new SkillFlow();
            switch (skillId)
            {
                case 101:
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    phases = skillPhaseBuilder.
                        AddMonsterCommonPhase().
                        AddSkillPreCastPhase(new SlimeSkillPreCastPhaseStrategy()).
                        AddSkillCastPhase(new SlimeSkillCastPhaseStrategy()).
                        AddSkillEventProcessPhase(new SlimeSkillEventProcessPhaseStrategy()).
                        AddSkillCastEndPhase(new SlimeSkillCastEndPhaseStrategy()).
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
