
namespace GameLogic.BattleMoudule.Relic
{
    /// <summary>
    /// 遗器套装效果接口
    /// </summary>
    public interface IRelicSetEffect : IComponent
    {
        /// <summary>
        /// 套装所属
        /// </summary>
        IBattleEntity Owner { get; }

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
        void SetOwner(IBattleEntity owner);

        /// <summary>
        /// 激活套装效果
        /// </summary>
        /// <param name="owner"></param>
        void Activate(IBattleEntity owner); 
    }
}
