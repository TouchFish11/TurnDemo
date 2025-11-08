
using UnityEngine.TextCore.Text;

namespace GameLogic.BattleMoudule.Summon
{
    /// <summary>
    /// 召唤物接口（与角色接口统一，复用战斗逻辑）
    /// </summary>
    public interface ISummon : IBattleEntity
    {
        /// <summary>
        /// 召唤者（主人）
        /// </summary>
        IBattleEntity Owner { get; }

        /// <summary>
        /// 初始化召唤物
        /// </summary>
        void Init(IBattleEntity owner);

        /// <summary>
        /// 剩余行动次数（配置表定义）(可选)
        /// </summary>
        //int RemainingActionTimes { get; }

        /// <summary>
        /// 消耗行动次数（API）(可选)
        /// </summary>
        //void ConsumeActionTime(); 
    }
}
