namespace HotUpdate.Game.Battle.Skill
{
    /// <summary>
    /// 技能类型
    /// </summary>
    public enum E_SkillType : byte
    {
        /// <summary>
        /// 怪物技能，怪物使用此类型
        /// </summary>
        Monster,
        
        /// <summary>
        /// 角色普攻
        /// </summary>
        NormalAttack,
        /// <summary>
        /// 角色战技
        /// </summary>
        CombatSkill,
        /// <summary>
        /// 角色终结技
        /// </summary>
        UltimateSkill,
        /// <summary>
        /// 强化普攻
        /// </summary>
        EnhancedNormalAttack,
        /// <summary>
        /// 强化战技
        /// </summary>
        EnhancedCombatSkill,
    }
}
