using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景管理器接口
/// </summary>
public interface ISceneManager
{
    /// <summary>
    /// 场景异步加载
    /// </summary>
    /// <param name="scenePath">场景路径</param>
    /// <param name="mode">加载模式</param>
    /// <param name="completed">结束回调</param>
    void LoadSceneAsync(string scenePath, LoadSceneMode mode, UnityAction<float> onLoadProgress, Func<Task> completed);
}
