
using GameLogic.BattleMoudule.Entity;
using System.Collections.Generic;

namespace GameLogic.BattleMoudule.Toughness
{
    /// <summary>
    /// 韧性组件接口
    /// </summary>
    public interface IToughnessComponent : IComponent
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void Init(IBattleEntity owner, List<E_PropertyType> weakPropertys, float initialToughness);
    }
}
