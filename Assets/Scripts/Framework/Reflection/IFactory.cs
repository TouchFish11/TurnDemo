
namespace Framework
{
    /// <summary>
    /// 工厂接口
    /// </summary>
    public interface IFactory
    {
        /// <summary>
        /// 初始化工厂
        /// </summary>
        void InitFactory();

        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        T GetValue<T>() where T : class;
    }
}
