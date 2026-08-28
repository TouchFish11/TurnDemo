using HotUpdate.Base.ECModule;
using HotUpdate.Game.Battle.Event.General;
using HotUpdate.Game.Battle.Object.Role;

namespace HotUpdate.Game.Battle.Property
{
    /// <summary>
    /// 角色属性组件
    /// </summary>
    [ComponentId]
    public class PlayerStatComponent : StatComponent
    {
        protected RoleProperty RoleProperty => battleProperty as RoleProperty;

        protected override void OnBattleInit()
        {
            battleProperty = new RoleProperty();
            ((RoleProperty)battleProperty).InitProperty(((IPlayerObject)BattleEntity).RoleInfo);
        }
        
    }
}
