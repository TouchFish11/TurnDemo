namespace Core.Reflection
{
    /// <summary>
    /// 工厂管理器接口
    /// </summary>
    public interface IFactoryManager
    {
        /// <summary>
        /// 初始化框架级工厂
        /// 通过反射创建所有工厂类型
        /// </summary>
        void InitHotFactorys();

        /// <summary>
        /// 获取工厂
        /// </summary>
        /// <typeparam name="TISubFactory">继承IFactory接口的子接口</typeparam>
        /// <typeparam name="TFactory">继承具体接口类型</typeparam>
        /// <returns></returns>
        TISubFactory GetFactory<TISubFactory, TFactory>() where TISubFactory : class, IFactory where TFactory : TISubFactory;

        /// <summary>
        /// 初始化框架工厂
        /// </summary>
        void InitCoreFactorys();
    }
}
