
namespace Framework
{
    /// <summary>
    /// 工厂接口
    /// </summary>
    public interface IFactory
    {
        /// <summary>
        /// 获取类型实例
        /// 子类可重写覆盖父类方法，实现自定义逻辑
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetTypeInstance<T>() where T : class;

        /// <summary>
        /// 初始化工厂
        /// </summary>
        void InitFactory();

        /// <summary>
        /// 获取类型实例
        /// 获取类型实例的唯一方法
        /// </summary>
        /// <typeparam name="TFactory"></typeparam>
        /// <typeparam name="TInstance"></typeparam>
        /// <returns></returns>
        static TInstance GetTypeInstance<TFactory, TInstance>() where TFactory : class, IFactory where TInstance : class
        { 
            return ServiceLocator.Get<IFactoryManager>().GetFactory<TFactory>().GetTypeInstance<TInstance>();
        }
    }
}
