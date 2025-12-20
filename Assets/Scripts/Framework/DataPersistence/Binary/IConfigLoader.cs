using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 配置加载器接口
/// </summary>
public interface IConfigLoader
{
    /// <summary>
    /// 加载本地配置
    /// </summary>
    /// <returns></returns>
    Task LoadConfig();

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T GetConfig<T>() where T : class;

}
