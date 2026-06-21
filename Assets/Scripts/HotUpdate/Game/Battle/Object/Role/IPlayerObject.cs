using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Battle.Command;
using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Object.Role
{
    public interface IPlayerObject : IBattleEntityObject
    {
        void RoleBattleInit(RoleInfo info, IBattleContext context, Commandfactory factory, IDeathHandler handler);

        /// <summary>
        /// 角色信息
        /// </summary>
        RoleInfo RoleInfo { get; }
    }
}
