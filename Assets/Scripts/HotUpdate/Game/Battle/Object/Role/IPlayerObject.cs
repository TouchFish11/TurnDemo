

namespace HotUpdate.Game.Battle.Object.Role
{
    public interface IPlayerObject : IBattleEntityObject
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
        /// <param name="initData"></param>
        void RoleBattleInit(RoleBattleInitData initData);
        
        /// <summary>
        /// 恢复终结技
        /// </summary>
        /// <param name="value"></param>
        void RecoverUltimate(int value);

        /// <summary>
        /// 设置角色信息
        /// </summary>
        /// <param name="roleInfo"></param>
        void SetRoleInfo(RoleInfo roleInfo);
    }
}
