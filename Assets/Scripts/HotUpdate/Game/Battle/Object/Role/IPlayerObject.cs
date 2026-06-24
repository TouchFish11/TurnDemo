using HotUpdate.Common.Config.ExcelInfo.Info;

namespace HotUpdate.Game.Battle.Object.Role
{
    public interface IPlayerObject : IBattleEntityObject
    {
        /// <summary>
        /// 角色信息
        /// </summary>
        RoleInfo RoleInfo { get; }
        
        /// <summary>
        /// 角色战斗初始化
        /// </summary>
        /// <param name="initData"></param>
        void RoleBattleInit(RoleBattleInitData initData);
    }
}
