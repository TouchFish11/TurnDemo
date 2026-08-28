using HotUpdate.Game.Battle.Property.New.Test.StatSystem.Resources;

namespace HotUpdate.Game.Battle.Property.New.Test.StatSystem
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
