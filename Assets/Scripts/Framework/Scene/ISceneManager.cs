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
    void LoadSceneAsync(string scenePath, LoadSceneMode mode, UnityAction<float> onLoadProgress, Func<Task> completed);
}
