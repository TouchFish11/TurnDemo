using HotUpdate.Game.Battle.StatSystem.Resources;

namespace HotUpdate.Game.Battle.StatSystem
{
    /// <summary>
    /// 角色状态集合
    /// </summary>
    public class RoleStatSet : StatSet
    {
        /// <summary>
        /// 角色释放终结技的资源
        /// </summary>
        public IResource UltimateResource { get; set; }
    }
}
