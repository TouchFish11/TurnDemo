using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 退出处理器接口
/// </summary>
public interface IQuitHandler
{
    event Func<Task> OnAppQuit;

    void ActiveHandler();
}
