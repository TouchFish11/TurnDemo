using HotUpdate.Base.Component;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗组件接口
    /// </summary>
    public interface IBattleComponent : IComponent
    {
        /// <summary>
        /// 战斗实体
        /// </summary>
        IBattleEntityObject BattleEntity { get; }
    }
}
