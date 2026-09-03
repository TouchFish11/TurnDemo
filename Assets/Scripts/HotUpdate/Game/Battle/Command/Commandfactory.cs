using Core.DI;
using Core.Pool;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Command
{
    /// <summary>
    /// 指令工厂。获取战斗指令
    /// </summary>
    public class Commandfactory
    {
        [Inject] private IPoolManager poolManager;
        
        /// <summary>
        /// 获取玩家角色的技能指令
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public SkillCommand GetSkillCommand(ISkill skill)
        {
            var skillCommand = poolManager.GetData<SkillCommand>();
            skillCommand.Init(skill);
            return skillCommand;
        }

        public MonsterActionCommand GetMonsterActionCommand(SkillCommand skillCommand)
        {
            var monsterActionCommand = poolManager.GetData<MonsterActionCommand>();
            monsterActionCommand.Init((IMonsterObject)skillCommand.Sender, skillCommand);
            return monsterActionCommand;
        }

        public TurnStartCommand GetTurnStartCommand(IBattleEntityObject battleEntity)
        {
            var turnStartCommand = poolManager.GetData<TurnStartCommand>();
            turnStartCommand.Init(battleEntity);
            return turnStartCommand;
        }
        
        //...
    }
}
