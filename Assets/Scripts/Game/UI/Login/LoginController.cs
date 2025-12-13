using Framework;
using System.Threading.Tasks;

/// <summary>
/// 登录界面控制器工厂
/// </summary>
public class LoginControllerFactory : UIControllerFactory<LoginView, LoginModel, LoginController>
{
    public override LoginModel CreateModel()
    {
        LoginModel loginModel = new LoginModel();
        return loginModel;
    }

    public override LoginController CreateController(LoginView view, LoginModel model)
    {
        return new LoginController(view, model);
    }
}

/// <summary>
/// 登录界面控制器
/// </summary>
public class LoginController : UIController<LoginView, LoginModel>
{
    private ILoginService _loginService;

    public LoginController(LoginView view, LoginModel model) : base(view, model)
    {

    }

    protected override async Task OnInit()
    {
        // 获取登录服务实例
        _loginService = ServerManager.Instance.GetService<LoginService>();
        // 注册自动登录完成事件
        _loginService.OnAutoLoginCompleted += OnAutoLoginCompleted;
        // 初始化登录数据
        _model.LoginData = _loginService.LoadLoginData();
        _model.IsLoginBtnEnabled = true;
        // 隐藏登录框
        ShowLoginBox(false);

        await base.OnInit();
    }

    protected override void ButtonOnClick(string btnName)
    {
        switch (btnName)
        {
            case "btnLogin":
                OnLoginClick();
                break;
            case "btnClose":
                UIManager.Instance.HideView<LoginView, LoginModel, LoginController>();
                break;
        }
    }

    protected override void InputFieldValueChanged(string fieldName, string inputStr)
    {
        switch (fieldName)
        {
            case "inputAccount":
                _model.SetAccount(inputStr);
                break;
            case "inputPassword":
                _model.SetPassword(inputStr);
                break;
        }
    }

    /// <summary>
    /// 显示登录框
    /// </summary>
    public void ShowLoginBox(bool isShow)
    {
        _model.IsActiveLoginBox = isShow;
    }

    /// <summary>
    /// 获取登录数据
    /// </summary>
    /// <returns></returns>
    public LoginData GetLoginData()
    {
        return _model.LoginData;
    }

    private void OnAutoLoginCompleted(bool result)
    {
        if (result)
        {
            // 自动登录成功，保存账号
            _loginService.SaveLoginData(_model.LoginData);
            // 开始检查更新
            LoginOver();
        }
        else
        {
            LogManager.Log($"登录失败");
            // 恢复按钮可用
            _model.IsLoginBtnEnabled = true;
            // 自动登录失败，手动登录，显示登录框
            ShowLoginBox(true);
        }
    }

    /// <summary>
    /// 登录结束
    /// </summary>
    private async void LoginOver()
    {
        LogManager.Log($"登录成功");
        // 隐藏登录界面
        UIManager.Instance.HideView<LoginView, LoginModel, LoginController>();
        // 显示开始界面
        BeginController beginController = await UIManager.Instance.ShowViewAsync<BeginView, BeginModel, BeginController>(E_UILayer.Mid);
        // 检查更新
        await beginController.CheckUpdate();
    }

    private async void OnLoginClick()
    {
        // 数据校验（调用 Model 方法）
        if (!_model.CheckLoginData())
        {
            LogManager.Log("账号或密码格式错误");
            return;
        }

        // 禁用登录按钮（修改 Model 数据 → 自动更新 View）
        _model.IsLoginBtnEnabled = false;

        // 调用外部服务（网络请求）
        await _loginService.LoginAsync(_model.LoginData);
    }
}
