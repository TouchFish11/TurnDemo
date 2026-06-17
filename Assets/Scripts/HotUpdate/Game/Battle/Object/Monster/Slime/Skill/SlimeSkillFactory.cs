using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Object.Monster.Slime.Strategys;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Handler;
using HotUpdate.Game.Battle.Skill.Nodes;

namespace HotUpdate.Game.Battle.Object.Monster.Slime.Skill
{
    /// <summary>
    /// Slime技能工厂
    /// </summary>
    public class SlimeSkillFactory : SkillFactory
    {
        /// <summary>
        /// 普攻动画状态名称
        /// 当前仅用于普攻技能的动画判断
        /// </summary>
        public static string Attack => "Attack";
        
        
        
        protected override SKillBuildData CreateSKillBuildData(int skillId)
        {
            SKillBuildData sKillBuildData = default;
            switch (skillId)
            {
                case 101:
                    var handler = skillCastPostHandlerFactory.GetSkillCastPostHandler<BaseSkillCastPostHandler>();
                    var effects = SkillNodeBuildPipeline.
                        AddMonsterPreNode().
                        AddTargetSelectNode().
                        AddProjectileInitNode(new SlimeProjectileInitStrategy()).
                        AddUpdateCameraNode(new SlimeUpdateCameraStrategy()).
                        AddDelayNode(0.1f).
                        AddPlayAnimationNode(AnimationUtility.Skill_Layer_Name, Attack, 0.9f).
                        AddCreateProjectileNode(AssetKeys.VFX_MonsterAttackSkill).
                        AddNode<ProcessProjectileEventNode>().
                        AddDelayNode(0.1f).
                        Build();
                    
                    sKillBuildData = new SKillBuildData(handler, effects);
                    break;
            }

            return sKillBuildData;
        }
    }
}
