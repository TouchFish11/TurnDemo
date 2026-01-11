using Framework;
using System;
using System.Collections.Generic;

/// <summary>
/// 工厂管理器
/// 管理工厂的初始化和获取
/// </summary>
public class FactoryManager : SingletonBase<FactoryManager> , IFactoryManager
{
    private readonly Dictionary<Type, IFactory> typeToFactoryMap = new Dictionary<Type, IFactory>();

    private FactoryManager()
    {

    }

    public void InitFactorys()
    {
        FactoryUtility.ScanFactorys(typeToFactoryMap);
    }

    public TFactory GetFactory<TFactory>() where TFactory : class, IFactory
    {
        if (typeToFactoryMap.TryGetValue(typeof(TFactory), out var factory))
        {
            return factory as TFactory;
        }

        LogManager.LogError($"未找到该工厂类型，{typeof(TFactory)}");
        return null;
    }
}
