using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.TargetSelect;

namespace HotUpdate.Game.Battle.Object
{
    /// <summary>
    /// 战斗角色初始化数据
    /// </summary>
    public class BattleObjectInitData
    {
        // 战斗实体ID
        public int BattleEntityId { get; set; }
        // 战斗上下文
        public IBattleContext BattleContext { get; set; }
        // 死亡处理器
        public IDeathHandler DeathHandler { get; set; }
        // 技能释放条件工厂
        public ICastSkillConditionFactory CastSkillConditionFactory { get; set; }
        // 目标选择策略工厂
        public ITargetSelectStrategyFactory TargetSelectStrategyFactory { get; set; }
        // 命令工厂
        public Commandfactory Commandfactory { get; set; }
    }
}
