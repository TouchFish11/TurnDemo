using Framework;
using Game.Battle;

/// <summary>
/// 状态工厂
/// 用于统一获取状态对象
/// </summary>
[FactoryType]
public class StatusFactory : IFactory
{
    void IFactory.InitFactory()
    {

    }

    /// <summary>
    /// 获取状态
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetStatus<T>(IBattleEntityObject sorucer, IBattleEntityObject owner, int statusId) where T : class, IPoolData, IStatus, new()
    {
        // 缓存池获取
        T status = PoolManager.Instance.GetData<T>();
        // 初始化状态
        status.InitStatus(sorucer, owner, statusId);
        return status;
    }

    T IFactory.GetValue<T>() where T : class
    {
        return null;
    }

    /// <summary>
    /// 回收状态
    /// 用于复用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    public void CollectStatus<T>(T data) where T : class, IPoolData, new()
    {
        PoolManager.Instance.PushData(data);
    }
}
