namespace HotUpdate.Base.Battle.Skill
{
    /// <summary>
    /// 技能目标类型
    /// </summary>
    public enum E_SkillTargetType : byte
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        
        /// <summary>
        /// 友方
        /// </summary>
        Friend = 1,
        
        /// <summary>
        /// 敌方
        /// </summary>
        Enemy,
    }
}
