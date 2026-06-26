using Core.DI;
using Core.Pool;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Toughness;

namespace HotUpdate.Game.Battle.Command
{
    /// <summary>
    /// 指令工厂。获取战斗指令
    /// </summary>
    public class Commandfactory
    {
        [Inject] private IPoolManager poolManager;
        
        /// <summary>
        /// 获取技能指令
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public SkillCommand GetSkillCommand(ISkill skill)
        {
            var skillCommand = poolManager.GetData<SkillCommand>();
            skillCommand.Init(skill);
            return skillCommand;
        }

        /// <summary>
        /// 获取怪物行动指令
        /// </summary>
        /// <param name="component"></param>
        /// <param name="skill"></param>
        /// <returns></returns>
        public MonsterActCommand GetMonsterActCommand(ToughnessComponent component, ISkill skill)
        {
            var skillCommand = poolManager.GetData<SkillCommand>();
            skillCommand.Init(skill);
            var command = poolManager.GetData<MonsterActCommand>();
            command.Init(component, skillCommand);
            return command;
        }
        
        // ...
    }
}
