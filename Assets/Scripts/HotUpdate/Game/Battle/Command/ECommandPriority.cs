namespace HotUpdate.Game.Battle.Command
{
    /// <summary>
    /// 命令类型优先级
    /// </summary>
    public enum ECommandPriority
    {
        /// <summary>
        /// 回合开始命令使用
        /// </summary>
        TurnStart,
        
        /// <summary>
        /// 怪物韧性恢复行动
        /// </summary>
        MonsterActionWithToughnessRecovery = 1,
        
        /// <summary>
        /// 怪物特殊技能
        /// </summary>
        MonsterSpecialSkill,
        
        /// <summary>
        /// 场外技
        /// </summary>
        OtcSkill,
        
        /// <summary>
        /// 追加攻击
        /// </summary>
        AdditionalAttack,
        
        /// <summary>
        /// 终结技
        /// </summary>
        Ultimate,
        
        /// <summary>
        /// 玩家特殊技能
        /// </summary>
        SpecialSkill,
        
        /// <summary>
        /// 角色技能
        /// </summary>
        RoleSkill,
        
        /// <summary>
        /// 怪物技能
        /// </summary>
        MonsterSkill,
    }
}
