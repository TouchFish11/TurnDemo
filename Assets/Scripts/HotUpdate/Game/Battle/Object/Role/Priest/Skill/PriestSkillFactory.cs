using HotUpdate.Game.Battle.Object.Monster.Slime.Effects;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Handler;
using HotUpdate.Game.Battle.Skill.Nodes;

namespace HotUpdate.Game.Battle.Object.Role.Priest.Skill
{
    /// <summary>
    /// 牧师技能工厂
    /// </summary>
    public class PriestSkillFactory : SkillFactory
    {
        // 动画状态名称常量：攻击状态（与Animator中状态名对应）
        private const string BattleAttackState = "BattleAttack";
        
        protected override SKillBuildData CreateSKillBuildData(int skillId)
        {
            SKillBuildData sKillBuildData = default;
            ISkillCastPostHandler handler;
            switch (skillId)
            {
                case 30:
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    var effects = SkillNodeBuildPipeline.
                        AddNode<TargetSelectNode>().
                        AddNode<SkillPointCastNode>().
                        AddNode<ProjectileInitNode>().
                        AddNode<PlayAnimationNode>(BattleAttackState, 0.2f).
                        AddNode<CreateProjectileNode>().
                        AddNode<ProcessProjectileEventNode>().
                        AddNode<DelayNode>(0.1f).
                        Build();
                    
                    sKillBuildData = new SKillBuildData(handler, effects);
                    break;
                case 31:
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    break;
                case 32:
                    handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>();
                    break;
            }

            return sKillBuildData;
        }
    }
}
