using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 二进制数据管理器接口
/// </summary>
public interface IBinaryDataManager
{
    T GetConfig<T>(E_ConfigLoadType loadType) where T : class;
    T Load<T>(string fileName) where T : new();
    Task LoadConfig();
    void Save(string fileName, object obj);
}
