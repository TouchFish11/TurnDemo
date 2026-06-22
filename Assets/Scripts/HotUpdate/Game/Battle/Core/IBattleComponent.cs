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

        /// <summary>
        /// 战斗初始化
        /// </summary>
        void BattleInit(IBattleEntityObject battleEntity);
        
        /// <summary>
        /// 战斗组件销毁逻辑
        /// </summary>
        /// <param name="battleEntity"></param>
        void DestroyBattle(IBattleEntityObject battleEntity);
    }
}
