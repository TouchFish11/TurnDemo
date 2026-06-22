using HotUpdate.Common.Config.ExcelInfo.Info;

namespace HotUpdate.Game.Battle.Object.Role
{
    public interface IPlayerObject : IBattleEntityObject
    {
        void RoleBattleInit(RoleBattleInitData initData);

        /// <summary>
        /// 角色信息
        /// </summary>
        RoleInfo RoleInfo { get; }
    }
}
