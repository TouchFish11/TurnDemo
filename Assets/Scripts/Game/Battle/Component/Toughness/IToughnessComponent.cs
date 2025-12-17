
using GameLogic.BattleMoudule.Entity;
using System.Collections.Generic;

namespace Game.Battle
{
    /// <summary>
    /// 韧性组件接口
    /// </summary>
    public interface IToughnessComponent
    {
        /// <summary>
        /// 初始化韧性组件
        /// </summary>
        void Init(IBattleEntityObject owner, List<E_PropertyType> weakPropertys, float initialToughness);
    }
}
