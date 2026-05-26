using HotUpdate.Base.Factory;

namespace HotUpdate.Game.Battle.TargetSelect
{
    public interface ITargetSelectStrategyFactory : IFactory
    {
        /// <summary>
        /// 获取目标选择策略
        /// </summary>
        /// <typeparam name="TStrategy">技能释放后处理器类型</typeparam>
        /// <returns></returns>
        ITargetSelectStrategy GetTargetSelectStrategy<TStrategy>()where TStrategy : class, ITargetSelectStrategy;
    }
}
