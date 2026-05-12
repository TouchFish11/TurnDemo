using Core.DI;
using Core.Pool;
using Core.Reflection;
using HotUpdate.Base.Battle.Command;
using HotUpdate.Base.Battle.Skill;
using HotUpdate.Base.Battle.Toughness;

namespace HotUpdate.Game.Battle.Command
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
            var skillCommand = DIContainer.GetInstance<IPoolManager>().GetData<SkillCommand>();
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
            var skillCommand = DIContainer.GetInstance<IPoolManager>().GetData<SkillCommand>();
            skillCommand.Init(skillData);
            var command = DIContainer.GetInstance<IPoolManager>().GetData<MonsterActCommand>();
            command.Init(component, skillCommand);
            return command;
        }
        
        // ...
    }
}
