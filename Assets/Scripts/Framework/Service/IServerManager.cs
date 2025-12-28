using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 服务管理器接口
/// </summary>
public interface IServerManager
{
    ILoginService GetService<T>() where T : class, new();
    LoginData LoadLoginData();
    Task TryAutoLogin();
}
