namespace HotUpdate.Game.Battle.Command
{
    /// <summary>
    /// 指令类型优先级
    /// </summary>
    public enum ECommandPriority
    {
        /// <summary>
        /// 韧性恢复
        /// </summary>
        MonsterSkillWithToughnessRecovery = 1,
        
        /// <summary>
        /// 场外技
        /// </summary>
        OtcSkill,
        
        /// <summary>
        /// 怪物特殊技能
        /// </summary>
        MonsterSpecialSkill,
        
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
