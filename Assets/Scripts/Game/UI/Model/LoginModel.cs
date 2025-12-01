using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 登录界面数据
/// </summary>
public class LoginModel : UIModel
{
    // 登录数据
    private LoginData loginData;
    // 登录按钮是否启用
    private bool isLoginBtnEnabled = true;
    // 登录框是否激活
    private bool isActiveLoginBox = false;

    /// <summary>
    /// 最小密码长度
    /// </summary>
    private const int MinPasswordLength = 6;

    public LoginData LoginData
    {
        get => loginData;
        set
        {
            loginData = value;
            TriggerDataChanged(nameof(loginData), value);
        }
    }

    /// <summary>
    /// 登录按钮是否启用
    /// </summary>
    public bool IsLoginBtnEnabled
    {
        get => isLoginBtnEnabled;
        set
        {
            isLoginBtnEnabled = value;
            TriggerDataChanged(nameof(isLoginBtnEnabled), value);
        }
    }

    public bool IsActiveLoginBox
    {
        get => isActiveLoginBox;
        set
        {
            isActiveLoginBox = value;
            TriggerDataChanged(nameof(isActiveLoginBox), value);
        }
    }

    /// <summary>
    /// 设置账号
    /// </summary>
    /// <param name="account"></param>
    public void SetAccount(string account)
    {
        loginData.account = account;
        TriggerDataChanged(nameof(loginData.account), account);
    }

    /// <summary>
    /// 设置密码
    /// </summary>
    /// <param name="password"></param>
    public void SetPassword(string password)
    {
        loginData.password = password;
        TriggerDataChanged(nameof(loginData.password), password);
    }

    /// <summary>
    /// 校验登录数据（Model 只做数据校验，不做网络请求）
    /// </summary>
    /// <returns></returns>
    public bool CheckLoginData()
    {
        return !string.IsNullOrEmpty(loginData.account) && loginData.password.Length >= MinPasswordLength;
    }
}
