using Core.Pool;
using Core.Reflection;
using Core.Service;
using HotUpdate.Battle.Toughness;
using HotUpdate.Core.Battle.Command;
using HotUpdate.Core.Battle.Skill;
using HotUpdate.Core.Battle.Toughness;

namespace HotUpdate.Battle.Command
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
        /// 获取怪物行动指令
        /// </summary>
        /// <param name="component"></param>
        /// <param name="skillData"></param>
        /// <returns></returns>
        public IMonsterActCommand GetMonsterActCommand(IToughnessComponent component, ISkillData skillData)
        {
            var skillCommand = ServiceLocator.Get<IPoolManager>().GetData<SkillCommand>();
            skillCommand.Init(skillData);
            var command = ServiceLocator.Get<IPoolManager>().GetData<MonsterActCommand>();
            command.Init(component, skillCommand);
            return command;
        }
        
        // ...
    }
}
