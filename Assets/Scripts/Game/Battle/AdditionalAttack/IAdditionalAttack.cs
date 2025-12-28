
namespace Game.Battle
{
    /// <summary>
    /// 追加攻击接口（所有支持追加攻击的模块实现此接口）
    /// </summary>
    public interface IAdditionalAttack
    {
        /// <summary>
        /// 是否满足触发条件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attacker"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        bool CanTrigger(IBattleContext context, IBattleEntityObject attacker, IBattleEntityObject target);

        /// <summary>
        /// 执行追加攻击
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attacker"></param>
        /// <param name="target"></param>
        void Execute(IBattleContext context, IBattleEntityObject attacker, IBattleEntityObject target);
    }
}
