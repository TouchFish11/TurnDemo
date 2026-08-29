namespace HotUpdate.Game.Battle.Object.Role
{
    /// <summary>
    /// 角色对象接口
    /// </summary>
    public interface IRoleObject : IBattleEntityObject
    {
        /// <summary>
        /// 角色信息
        /// </summary>
        RoleInfo RoleInfo { get; }
        
        /// <summary>
        /// 当前角色所处的行动阶段
        /// </summary>
        EActPhase CurrentActPhase { get; set; }
        
        /// <summary>
        /// 角色战斗初始化
        /// </summary>
        /// <param name="parameter"></param>
        void RoleBattleInit(BattleParameterObject parameter);

        /// <summary>
        /// 恢复终结技所需资源
        /// </summary>
        /// <param name="value"></param>
        void RecoverUltimate(float value);

        /// <summary>
        /// 设置角色信息
        /// </summary>
        /// <param name="roleInfo"></param>
        void SetRoleInfo(RoleInfo roleInfo);
    }
}
