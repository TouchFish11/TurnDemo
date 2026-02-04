using Core.Pool;
using Core.Reflection;
using Core.Service;
using Game.Battle.Command;
using Game.Battle.Skill.Interface;
using Game.Battle.Toughness;

namespace GameHotUpdate.Battle.Command
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
        /// <param name="skillData"></param>
        /// <returns></returns>
        public ISkillCommand GetSkillCommand(ISkillData skillData)
        {
            var skillCommand = ServiceLocator.Get<IPoolManager>().GetData<SkillCommand>();
            skillCommand.Init(skillData);
            return skillCommand;
        }

        /// <summary>
        /// 获取韧性指令
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public IToughnessCommand GetToughnessCommand(IToughnessComponent component)
        {
            var command = ServiceLocator.Get<IPoolManager>().GetData<ToughnessCommand>();
            command.Init(component);
            return command;
        }
        
        // ...
    }
}
