using Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 退出处理器
/// </summary>
public class QuitHandler : SingletonAutoMono<QuitHandler>
{
    /// <summary>
    /// 在应用程序退出时
    /// </summary>
    public event Func<Task> OnAppQuit;

    /// <summary>
    /// 初始化退出处理器
    /// </summary>
    public void ActiveHandler()
    {
        LogManager.Instance.EnableLog = true;
        LogManager.Log(Application.persistentDataPath);
        LogManager.Log("退出处理器激活");
    }

    private async void OnApplicationQuit()
    {
        await OnAppQuit?.Invoke();
        OnAppQuit = null;
    }
}
