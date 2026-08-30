namespace HotUpdate.Game.Battle.StatSystem.Resources
{
    /// <summary>
    /// 特殊属性资源
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// 特殊资源属性类型
        /// </summary>
        EResourceStatType ResourceStatType { get; }
        
        /// <summary>
        /// 当前值
        /// </summary>
        float CurrentValue { get; }
        
        /// <summary>
        /// 最大值
        /// </summary>
        float MaxValue { get; }
        
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="amount">获取量</param>
        void Gain(float amount);
        
        /// <summary>
        /// 消耗
        /// </summary>
        /// <param name="amount">消耗量</param>
        void Consume(float amount);
    }
}
