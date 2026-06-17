using System;
using Core.DI;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Object.Monster.Slime.Effects;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Handler;
using HotUpdate.Game.Battle.Skill.Nodes;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill
{
    /// <summary>
    /// 战士技能工厂
    /// </summary>
    public class WarriorSkillFactory : SkillFactory
    {
        protected override SKillBuildData CreateSKillBuildData(int skillId)
        {
            SKillBuildData sKillBuildData = default;
            switch (skillId)
            {
                case 10:
                    var handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    var effects = SkillNodeBuildPipeline.
                        AddNode<MonsterPreNode>().
                        AddNode<TargetSelectNode>().
                        AddNode<SlimeProjectileInitNode>().
                        AddNode<UpdateCameraNode>(stragty).
                        AddNode<DelayNode>(0.1f).
                        AddNode<PlayAnimationNode>(stateName, targetEndProgress).
                        AddNode<CreateProjectileNode>().
                        AddNode<ProcessProjectileEventNode>().
                        AddNode<DelayNode>(0.1f).
                        Build();
                    
                    sKillBuildData = new SKillBuildData(handler, effects);
                    break;
                case 11:
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    var warriorBattleSkill = DIContainer.Create<WarriorBattleSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(warriorBattleSkill, handler);
                case 12:
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    var warriorUltimateSkill = DIContainer.Create<WarriorUltimateSkill>(parameterValues: new object[] { caster, skillId });
                    return new SkillData(warriorUltimateSkill, handler);
            }

            return sKillBuildData;
        }
    }
}
