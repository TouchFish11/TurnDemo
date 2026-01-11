using Framework;
using Game.Battle;
using System;
using System.Collections.Generic;

/// <summary>
/// 状态工厂
/// 用于统一获取状态对象
/// </summary>
public class StatusFactory : IFactory
{
    private readonly Dictionary<int, Type> idToTypeMap = new Dictionary<int, Type>();

    void IFactory.InitFactory()
    {
        FactoryUtility.ScanAllStatu(idToTypeMap);
    }
    T IFactory.GetTypeInstance<T>()
    {
        return null;
    }

    /// <summary>
    /// 获取状态
    /// </summary>
    /// <returns></returns>
    public IStatus GetStatus(int statusId)
    {
        if (idToTypeMap.TryGetValue(statusId, out Type statusType))
        {
            // 缓存池获取
            return Activator.CreateInstance(statusType) as IStatus;
        }

        return null;
    }
}
