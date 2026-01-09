using Framework;

/// <summary>
/// 工厂管理器接口
/// </summary>
public interface IFactoryManager
{
    /// <summary>
    /// 初始化工厂
    /// </summary>
    void InitFactorys();

    /// <summary>
    /// 获取工厂
    /// </summary>
    /// <typeparam name="TFactory">继承IFactory接口</typeparam>
    /// <returns></returns>
    TFactory GetFactory<TFactory>() where TFactory : class, IFactory;

}
