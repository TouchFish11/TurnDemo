using HotUpdate.Base.Factory;

namespace HotUpdate.Game.Battle.Toughness
{
    public interface IToughnessStrategyFactory : IFactory
    {
        /// <summary>
        /// 获取指定类型的韧性削减策略实例
        /// </summary>
        /// <typeparam name="T">目标削减策略类型（需实现IToughnessReduceStrategy）</typeparam>
        /// <returns>匹配的削减策略实例，未找到则返回null</returns>
        IToughnessReduceStrategy GetReduceStrategy<T>() where T : class, IToughnessReduceStrategy;
        
        /// <summary>
        /// 获取指定类型的韧性数值计算策略实例
        /// </summary>
        /// <typeparam name="T">目标数值计算策略类型（需实现IToughnessCalcStrategy）</typeparam>
        /// <returns>匹配的数值计算策略实例，未找到则返回null</returns>
        IToughnessCalcStrategy GetCalcStrategy<T>() where T : class, IToughnessCalcStrategy;
    }
}
