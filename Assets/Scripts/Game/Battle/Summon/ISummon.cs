
namespace Game.Battle
{
    /// <summary>
    /// 召唤物接口（复用战斗逻辑）
    /// </summary>
    public interface ISummon : IBattleEntityObject
    {
        /// <summary>
        /// 召唤者（主人）
        /// </summary>
        IBattleEntityObject Owner { get; }

        /// <summary>
        /// 初始化召唤物
        /// </summary>
        void Init(IBattleEntityObject owner);

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
