using System.Collections.Generic;
using Core.DI;
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
                        AddSkillPreCastPhase(DIContainer.Create<SlimeSkillPreCastPhaseStrategy>()).
                        AddSkillCastPhase(DIContainer.Create<SlimeSkillCastPhaseStrategy>()).
                        AddSkillEventProcessPhase(DIContainer.Create<SlimeSkillEventProcessPhaseStrategy>()).
                        AddSkillCastEndPhase(DIContainer.Create<SlimeSkillCastEndPhaseStrategy>()).
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
