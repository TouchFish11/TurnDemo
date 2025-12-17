

namespace Game.Battle
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
    }
}
