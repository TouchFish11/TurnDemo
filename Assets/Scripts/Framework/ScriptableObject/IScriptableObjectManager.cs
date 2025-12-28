using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可脚本化管理器接口
/// </summary>
public interface IScriptableObjectManager
{
    T LoadSO<T>(string path) where T : ScriptableObject;
}
