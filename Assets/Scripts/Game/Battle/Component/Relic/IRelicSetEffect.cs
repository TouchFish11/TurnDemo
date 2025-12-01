
using GameLogic.BattleMoudule.Entity;

namespace GameLogic.BattleMoudule.Relic
{
    /// <summary>
    /// 遗器套装效果接口
    /// </summary>
    public interface IRelicSetEffect : IBattleComponent
    {
        /// <summary>
        /// 套装所属
        /// </summary>
        IBattleEntityObject Owner { get; }

        /// <summary>
        /// 套装名称
        /// </summary>
        string SetName { get; }

        /// <summary>
        /// 激活所需件数（如2件套、4件套）
        /// </summary>
        int RequiredCount { get; }

        /// <summary>
        /// 设置套装所有者
        /// </summary>
        /// <param name="owner"></param>
        void SetOwner(IBattleEntityObject owner);

        /// <summary>
        /// 激活套装效果
        /// </summary>
        /// <param name="owner"></param>
        void Activate(IBattleEntityObject owner); 
    }
}
