using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 服务管理器
/// </summary>
public class ServerManager : SingletonBase<ServerManager>
{
    private readonly Dictionary<Type, object> _typeToSeverMap = new Dictionary<Type, object>();

    private ServerManager()
    {
        _typeToSeverMap.Add(typeof(LoginService), new LoginService());
    }

    /// <summary>
    /// 获取登录服务实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public ILoginService GetService<T>() where T : class, new()
    {
        return _typeToSeverMap[typeof(T)] as ILoginService;
    }

    /// <summary>
    /// 尝试自动登录
    /// </summary>
    /// <returns></returns>
    public async Task TryAutoLogin()
    {
        LoginController loginController = await UIManager.Instance.ShowViewAsync<LoginView, LoginModel, LoginController>(E_UILayer.Mid);
        LoginData loginData = loginController.GetLoginData();

        // 校验缓存有效性
        if (VerifyValidity(loginData))
        {
            // 执行自动登录（复用普通登录逻辑）
            await (_typeToSeverMap[typeof(LoginService)] as ILoginService).LoginAsync(loginData);
        }
        // 没有本地缓存
        else
        {
            // 自动登录失败，显示登录框
            UIManager.Instance.GetView<LoginView, LoginModel, LoginController>().ShowLoginBox(true);
        }
    }

    /// <summary>
    /// 加载登录数据
    /// </summary>
    /// <returns></returns>
    public LoginData LoadLoginData()
    {
        return (_typeToSeverMap[typeof(LoginService)] as ILoginService).LoadLoginData();
    }

    /// <summary>
    /// 校验有效性
    /// </summary>
    /// <param name="loginData"></param>
    /// <returns></returns>
    private bool VerifyValidity(LoginData loginData)
    {
        // 校验缓存有效性
        if (string.IsNullOrEmpty(loginData.account) || string.IsNullOrEmpty(loginData.password))
        {
            LogManager.Log("无缓存的登录信息");
            return false;
        }
        return true;
    }
}