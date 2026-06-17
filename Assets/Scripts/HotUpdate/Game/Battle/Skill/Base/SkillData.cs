namespace HotUpdate.Game.Battle.Skill.Base
{
    public struct SkillData : ISkillData
    {
        public ISkill Skill { get; }

        public SkillData(ISkill skill)
        {
            Skill = skill;
        }
    }
}
