namespace Game.Battle.Skill.Interface
{
    public interface ISkillManager
    {
        /// <summary>
        /// 添加技能指令
        /// </summary>
        /// <param name="skilldata"></param>
        void AddSkillCommand(ISkillData skilldata);

        /// <summary>
        /// 初始化技能目标
        /// </summary>
        /// <param name="skill"></param>
        void InitSkillTarget(ISkill skill);
    }
}
