using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 登录服务接口
/// </summary>
public interface ILoginService
{
    /// <summary>
    /// 自动登录完成事件
    /// </summary>
    public event UnityAction<bool> OnAutoLoginCompleted;

    /// <summary>
    /// 异步登录
    /// </summary>
    /// <returns></returns>
    Task LoginAsync(LoginData loginData);

    /// <summary>
    /// 保存登录数据
    /// </summary>
    /// <param name="account"></param>
    /// <param name="password"></param>
    void SaveLoginData(LoginData loginData);

    /// <summary>
    /// 加载登录数据
    /// </summary>
    LoginData LoadLoginData();
}
