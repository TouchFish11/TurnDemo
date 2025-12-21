using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 实体对象接口
/// </summary>
public interface IEntityObject
{
    /// <summary>
    /// 游戏对象
    /// </summary>
    GameObject GameObject { get; }

    /// <summary>
    /// 基础初始化
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    void BaseInit(int id);

    /// <summary>
    /// 获取组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T GetComponent<T>() where T : Component;

    /// <summary>
    /// 获取子对象组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T GetComponentInChildren<T>() where T : Component;

    /// <summary>
    /// 添加组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    T AddComponent<T>() where T : Component;

    /// <summary>
    /// 批量添加组件
    /// </summary>
    /// <param name="componentIds"></param>
    /// <returns></returns>
    bool AddComponents(params int[] componentIds);
}
