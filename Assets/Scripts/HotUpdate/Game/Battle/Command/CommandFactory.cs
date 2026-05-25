using Core.DI;
using Core.Pool;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Toughness;

namespace HotUpdate.Game.Battle.Command
{
    /// <summary>
    /// 指令工厂。获取战斗指令
    /// </summary>
    public class CommandFactory
    {
        /// <summary>
        /// 获取技能指令
        /// </summary>
        /// <param name="skillData"></param>
        /// <returns></returns>
        public SkillCommand GetSkillCommand(ISkillData skillData)
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
        public MonsterActCommand GetMonsterActCommand(IToughnessComponent component, ISkillData skillData)
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
