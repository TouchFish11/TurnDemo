using System.Collections.Generic;
using HotUpdate.Common.Config;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object.Conditions;
using HotUpdate.Game.Battle.Property.New.Test.StatSystem;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.TargetSelect;

namespace HotUpdate.Game.Battle.Object
{
    /// <summary>
    /// 战斗角色初始化参数对象
    /// </summary>
    public class BattleParameterObject
    {
        /// <summary>
        /// 战斗实体ID
        /// </summary>
        public int BattleEntityId { get; set; }
        
        /// <summary>
        /// 战斗信息
        /// </summary>
        public IBattleInfo BattleInfo { get; set; }
        
        /// <summary>
        /// 对象使用的特殊的资源类型
        /// </summary>
        public EResourceStatType ResourceStatType { get; set; }
        
        /// <summary>
        /// 战斗上下文
        /// </summary>
        public IBattleContext BattleContext { get; set; }
        
        /// <summary>
        /// 死亡逻辑处理器
        /// </summary>
        public IDeathHandler DeathHandler { get; set; }
        
        /// <summary>
        /// 技能释放条件工厂
        /// </summary>
        public ICastSkillConditionFactory CastSkillConditionFactory { get; set; }
        
        /// <summary>
        /// 目标选择策略工厂
        /// </summary>
        public ITargetSelectStrategyFactory TargetSelectStrategyFactory { get; set; }
        
        /// <summary>
        /// 指令工厂
        /// </summary>
        public Commandfactory Commandfactory { get; set; }
        
        /// <summary>
        /// 对象死亡条件列表
        /// </summary>
        public List<IDeathCondition> DeathConditions { get; set; }
    }
}
