namespace Game.Battle.Skill.Interface
{
    public interface ISkillManager
    {
        /// <summary>
        /// 初始化技能目标
        /// </summary>
        /// <param name="skill"></param>
        void InitSkillTarget(ISkill skill);
    }
}
