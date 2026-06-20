using System.Collections.Generic;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.AbyssGift;
using HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.AbyssLock;
using HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.Ashfall;
using HotUpdate.Game.Battle.Object.Monster.AbyssalMage.Skill.Frostfall;
using HotUpdate.Game.Battle.Skill;
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
                    phases = SkillPhaseBuilder.
                        AddMonsterCommonPhase().
                        AddSkillPreCastPhase(new AbyssalMageFrostfallSkillPreCastPhaseStrategy()).
                        AddSkillCastPhase(new AbyssalMageFrostfallSkillCastPhaseStrategy()).
                        AddSkillEventProcessPhase(new AbyssalMageFrostfallSkillEventProcessPhaseStrategy()).
                        AddSkillCastEndPhase(new AbyssalMageFrostfallSkillCastEndPhaseStrategy()).
                        Build();
                    break;
                case 104:   // 烬陨
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    phases = SkillPhaseBuilder.
                        AddMonsterCommonPhase().
                        AddSkillPreCastPhase(new AbyssalMageAshfallSkillPreCastPhaseStrategy()).
                        AddSkillCastPhase(new AbyssalMageAshfallSkillCastPhaseStrategy()).
                        AddSkillEventProcessPhase(new AbyssalMageAshfallSkillEventProcessPhaseStrategy()).
                        AddSkillCastEndPhase(new AbyssalMageAshfallSkillCastEndPhaseStrategy()).
                        Build();
                    
                    effects = SkillPhaseBuilder.
                        AddMonsterPreNode().
                        AddTargetSelectNode().
                        AddProjectileInitNode(projectileInitStrategy.InitAttack).
                        AddUpdateCameraNode().
                        AddDelayNode(0.1f).
                        AddPlayAnimationNode(AnimationUtility.Skill_Layer_Name, Attack, 0.9f).
                        AddCreateProjectileNode(AssetKeys.VFX_MonsterAttackSkill).
                        AddProcessProjectileEventNode(strategy.PriestNormalSkillEvent).
                        AddDelayNode(0.1f).
                        Build();
                    break;
                case 105:   // 深渊之赐
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<AbyssalMageSkillCastPostHandler>();
                    phases = SkillPhaseBuilder.
                        AddMonsterCommonPhase().
                        AddSkillPreCastPhase(new AbyssalMageAbyssGiftSkillPreCastPhaseStrategy()).
                        AddSkillCastPhase(new AbyssalMageAbyssGiftSkillCastPhaseStrategy()).
                        AddSkillEventProcessPhase(new AbyssalMageAbyssGiftSkillEventProcessPhaseStrategy()).
                        Build();
                    break;
                case 106:   // 渊禁
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<AbyssalMageSkillCastPostHandler>();
                    phases = SkillPhaseBuilder.
                        AddMonsterCommonPhase().
                        AddSkillPreCastPhase(new AbyssalMageAbyssLockSkillPreCastPhaseStrategy()).
                        AddSkillCastPhase(new AbyssalMageAbyssLockSkillCastPhaseStrategy()).
                        AddSkillEventProcessPhase(new AbyssalMageAbyssLockSkillEventProcessPhaseStrategy()).
                        Build();
                    
                    effects = SkillPhaseBuilder.
                        AddMonsterPreNode().
                        AddTargetSelectNode().
                        AddProjectileInitNode(projectileInitStrategy.InitAttack).
                        AddUpdateCameraNode().
                        AddDelayNode(0.1f).
                        AddPlayAnimationNode(AnimationUtility.Skill_Layer_Name, Attack, 0.9f).
                        AddCreateProjectileNode(AssetKeys.VFX_MonsterAttackSkill).
                        AddProcessProjectileEventNode(strategy.PriestNormalSkillEvent).
                        AddDelayNode(0.1f).
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
