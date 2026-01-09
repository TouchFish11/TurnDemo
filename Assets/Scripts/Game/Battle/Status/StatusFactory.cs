using Framework;
using Game.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 状态工厂
/// 用于统一获取状态对象
/// </summary>
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
    public T GetValue<T>() where T : class, IPoolData, new()
    {
        // 缓存池获取
        return PoolManager.Instance.GetData<T>();
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
