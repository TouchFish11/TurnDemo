using Framework;
using Net.FrameSync;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 登录服务
/// </summary>
public class LoginService : ILoginService
{
    public event UnityAction<bool> OnAutoLoginCompleted;

    public async Task LoginAsync(LoginData loginData)
    {
        // 开启客户端
        NetManager.Instance.StartClient("127.0.0.1", 8080);
        //等待连接成功
        await TaskUtility.WaitUntil(() => NetManager.Instance.GetTcpClient().ConnectData != null);
        // 执行回调
        OnAutoLoginCompleted?.Invoke(NetManager.Instance.Connected);
    }

    public void SaveLoginData(LoginData loginData)
    {
        BinaryDataManager.Instance.Save(FileUtility.LocalLoginDataFileName, loginData);
    }

    public LoginData LoadLoginData()
    {
        return BinaryDataManager.Instance.Load<LoginData>(FileUtility.LocalLoginDataFileName);
    }
}
