
namespace Core.Pool
{
    /// <summary>
    /// 对象池数据接口
    /// 继承接口的纯C#类才能被缓存池管理
    /// </summary>
    public interface IPoolData
    {
        /// <summary>
        /// 重置数据
        /// </summary>
        void ResetData();
    }
}
