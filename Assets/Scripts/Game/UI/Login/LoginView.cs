using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 登录界面
/// </summary>
public class LoginView : UIView
{
    private InputField inputAccount;
    private InputField inputPassword;
    private Button btnLogin;
    private GameObject loginBox;

    protected override void Awake()
    {
        base.Awake();

        inputAccount = binder.GetControl<InputField>(nameof(inputAccount));
        inputPassword = binder.GetControl<InputField>(nameof(inputPassword));
        btnLogin = binder.GetControl<Button>(nameof(btnLogin));

        loginBox = this.transform.Find(nameof(loginBox)).gameObject;
    }


    /// <summary>
    /// 更新界面（接收 Controller 指令）
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
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
                loginBox.SetActive((bool)value);
                break;
        }
    }
}
