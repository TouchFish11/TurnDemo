namespace HotUpdate.Base.Factory
{
    /// <summary>
    /// 工厂管理器接口
    /// </summary>
    public interface IFactoryManager
    {
        /// <summary>
        /// 获取工厂
        /// </summary>
        /// <typeparam name="TISubFactory">继承IFactory接口的子接口</typeparam>
        /// <typeparam name="TFactory">继承具体接口类型</typeparam>
        /// <returns></returns>
        //TISubFactory GetFactory<TISubFactory, TFactory>() where TISubFactory : class, IFactory where TFactory : TISubFactory;

        void BindFactory();
    }
}
