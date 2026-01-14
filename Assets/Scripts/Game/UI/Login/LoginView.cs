using Framework;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 登录界面
/// </summary>
public class LoginView : UIView
{
    [Inject] private InputField inputAccount;
    [Inject] private InputField inputPassword;
    [Inject] private Button btnLogin;

    [Inject(1)] private RectTransform loginBox;

    /// <summary>
    /// 更新界面（接收 Controller 指令）
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    [System.Obsolete]
    public override void UpdateView(string key, object value)
    {
        switch (key)
        {
            case "account":
                inputAccount.text = value.ToString();
                break;
            case "password":
                inputPassword.text = value.ToString();
                break;
            case "isLoginBtnEnabled":
                btnLogin.interactable = (bool)value;
                break;
            case "isActiveLoginBox":
                loginBox.gameObject.SetActive((bool)value);
                break;
        }
    }
}
