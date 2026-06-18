using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Skill.Nodes
{
    /// <summary>
    /// 技能执行效果，可以为终结技效果和非终结技效果
    /// </summary>
    public abstract class SkillExecuteNode : SkillNode
    {
        
        protected SkillExecuteNode(ISkill skill) : base(skill)
        {
            
        }

        public override bool CanExecute()
        {
            return true;
        }
    }
}
