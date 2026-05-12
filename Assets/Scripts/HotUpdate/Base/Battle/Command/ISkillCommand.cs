using HotUpdate.Base.Battle.Skill;

namespace HotUpdate.Base.Battle.Command
{
    public interface ISkillCommand : ICommand
    {
        /// <summary>
        /// 技能数据
        /// </summary>
        ISkillData SkillData { get; }
        
        /// <summary>
        /// 初始化技能指令
        /// </summary>
        /// <param name="SkillData"></param>
        void Init(ISkillData SkillData);
    }
}
