using System.Collections.Generic;
using Core.DI;
using HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.AbyssGift;
using HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.AbyssLock;
using HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.Ashfall;
using HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.Frostfall;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.Battle.Skill.Base.Phase;
using HotUpdate.Game.Battle.Skill.Handler;

namespace HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill
{
    /// <summary>
    /// 深渊法师技能工厂
    /// </summary>
    public class AbyssalMageSkillFactory : SkillFactory
    {
        protected override SKillBuildData CreateSKillBuildData(int skillId)
        {
            ISkillCastPostHandler handler = null;
            List<ISkillFlowPhase> phases = null;
            var flow = new SkillFlow();
            
            switch (skillId)
            {
                case 103:   // 霜陨
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    phases = skillPhaseBuilder.
                        AddMonsterCommonPhase().
                        AddSkillPreCastPhase(DIContainer.Create<AbyssalMageFrostfallSkillPreCastPhaseStrategy>()).
                        AddSkillCastPhase(DIContainer.Create<AbyssalMageFrostfallSkillCastPhaseStrategy>()).
                        AddSkillEventProcessPhase(DIContainer.Create<AbyssalMageFrostfallSkillEventProcessPhaseStrategy>()).
                        AddSkillCastEndPhase(DIContainer.Create<AbyssalMageFrostfallSkillCastEndPhaseStrategy>()).
                        Build();
                    break;
                case 104:   // 烬陨
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    phases = skillPhaseBuilder.
                        AddMonsterCommonPhase().
                        AddSkillPreCastPhase(DIContainer.Create<AbyssalMageAshfallSkillPreCastPhaseStrategy>()).
                        AddSkillCastPhase(DIContainer.Create<AbyssalMageAshfallSkillCastPhaseStrategy>()).
                        AddSkillEventProcessPhase(DIContainer.Create<AbyssalMageAshfallSkillEventProcessPhaseStrategy>()).
                        AddSkillCastEndPhase(DIContainer.Create<AbyssalMageAshfallSkillCastEndPhaseStrategy>()).
                        Build();
                    break;
                case 105:   // 深渊之赐
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<AbyssalMageSkillCastPostHandler>();
                    phases = skillPhaseBuilder.
                        AddMonsterCommonPhase().
                        AddSkillPreCastPhase(DIContainer.Create<AbyssalMageAbyssGiftSkillPreCastPhaseStrategy>()).
                        AddSkillCastPhase(DIContainer.Create<AbyssalMageAbyssGiftSkillCastPhaseStrategy>()).
                        AddSkillEventProcessPhase(DIContainer.Create<AbyssalMageAbyssGiftSkillEventProcessPhaseStrategy>()).
                        AddSkillCastEndPhase(DIContainer.Create<AbyssalMageAbyssGiftSkillCastEndPhaseStrategy>()).
                        Build();
                    break;
                case 106:   // 渊禁
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<AbyssalMageSkillCastPostHandler>();
                    phases = skillPhaseBuilder.
                        AddMonsterCommonPhase().
                        AddSkillPreCastPhase(DIContainer.Create<AbyssalMageAbyssLockSkillPreCastPhaseStrategy>()).
                        AddSkillCastPhase(DIContainer.Create<AbyssalMageAbyssLockSkillCastPhaseStrategy>()).
                        AddSkillEventProcessPhase(DIContainer.Create<AbyssalMageAbyssLockSkillEventProcessPhaseStrategy>()).
                        AddSkillCastEndPhase(DIContainer.Create<AbyssalMageAbyssLockSkillCastEndPhaseStrategy>()).
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
