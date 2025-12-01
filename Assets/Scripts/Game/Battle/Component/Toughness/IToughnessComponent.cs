
using GameLogic.BattleMoudule.Entity;
using System.Collections.Generic;

namespace GameLogic.BattleMoudule.Toughness
{
    /// <summary>
    /// 韧性组件接口
    /// </summary>
    public interface IToughnessComponent : IBattleComponent
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void Init(IBattleEntityObject owner, List<E_PropertyType> weakPropertys, float initialToughness);
    }
}
