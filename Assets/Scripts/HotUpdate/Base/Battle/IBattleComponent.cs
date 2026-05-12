using Core.Components;
using HotUpdate.Base.Battle.Object;

namespace HotUpdate.Base.Battle
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

        /// <summary>
        /// 战斗初始化
        /// </summary>
        public void BattleInit(IBattleEntityObject battleEntity);
    }
}
