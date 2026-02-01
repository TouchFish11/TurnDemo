using Core.Pool;
using Core.Reflection;
using Game.Battle.Skill;
using Game.Battle.Toughness;

namespace Game.Battle.Command
{
    /// <summary>
    /// 指令工厂
    /// 获取战斗指令
    /// </summary>
    public class CommandFactory : ICommandFactory
    {
        void IFactory.InitFactory()
        {
            
        }

        /// <summary>
        /// 获取技能指令
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public ISkillCommand GetSkillCommand(ISkill skill)
        {
            var skillCommand = PoolManager.Instance.GetData<SkillCommand>();
            skillCommand.Init(skill);
            return skillCommand; 
        }

        /// <summary>
        /// 获取韧性指令
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public IToughnessCommand GetToughnessCommand(IToughnessComponent component)
        {
            var command = PoolManager.Instance.GetData<ToughnessCommand>();
            command.Init(component);
            return command;
        }
        
        // ...
    }
}
