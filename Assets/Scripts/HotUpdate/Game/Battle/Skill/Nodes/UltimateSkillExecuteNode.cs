namespace HotUpdate.Game.Battle.Skill.Nodes
{
    /// <summary>
    /// 玩家角色终结技技能执行效果，若是Pose就正常逻辑，动画就播放动画即可
    /// </summary>
    public abstract class UltimateSkillExecuteNode : SkillExecuteNode
    {
        protected UltimateSkillExecuteNode(ISkill skill) : base(skill)
        {
            
        }
    }
}
