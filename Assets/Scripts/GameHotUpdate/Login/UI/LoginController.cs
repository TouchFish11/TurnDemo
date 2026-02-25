using Core.Log;
using Core.Service;
using Core.Service.Login;
using Core.UI;
using Core.UI.MVC;
using GameHotUpdate.Config;

namespace GameHotUpdate.Login.UI
{
    // /// <summary>
    // /// ��¼�������������
    // /// </summary>
    // public class LoginControllerFactory : UIControllerFactory<LoginViewWrapper, LoginModel, LoginController>
    // {
    //     protected override LoginModel CreateModel()
    //     {
    //         return new LoginModel();
    //     }
    //
    //     protected override LoginController CreateController(LoginViewWrapper viewWrapper, LoginModel model)
    //     {
    //         return new LoginController(viewWrapper.UiInstance, model);
    //     }
    // }

    /// <summary>
    /// ��¼���������
    /// </summary>
    //[UIControllerFactory(typeof(LoginControllerFactory))]
    public class LoginController : UIController<LoginView, LoginModel>
    {
        private ILoginService _loginService;

        public LoginController(LoginView view, LoginModel model) : base()
        {

        }

        protected override System.Threading.Tasks.Task OnInit()
        {
            // ��ȡ��¼����ʵ��
            _loginService = ServerManager.Instance.GetService<LoginService>();
            // ע���Զ���¼����¼�
            _loginService.OnAutoLoginCompleted += OnAutoLoginCompleted;
            // ��ʼ����¼����
            model.LoginData = _loginService.LoadLoginData();
            model.IsLoginBtnEnabled = true;
            // ���ص�¼��
            ShowLoginBox(false);
            return System.Threading.Tasks.Task.CompletedTask;
        }

        protected override void ButtonOnClick(string btnName)
        {
            switch (btnName)
            {
                case "btnLogin":
                    OnLoginClick();
                    break;
                case "btnClose":
                    ServiceLocator.Get<IUIManager>().DestroyView(AbKeyCollection.Ui, this);
                    break;
            }
        }

        protected override void InputFieldValueChanged(string fieldName, string inputStr)
        {
            switch (fieldName)
            {
                case "inputAccount":
                    model.SetAccount(inputStr);
                    break;
                case "inputPassword":
                    model.SetPassword(inputStr);
                    break;
            }
        }

        /// <summary>
        /// ��ʾ��¼��
        /// </summary>
        public void ShowLoginBox(bool isShow)
        {
            model.IsActiveLoginBox = isShow;
        }

        /// <summary>
        /// ��ȡ��¼����
        /// </summary>
        /// <returns></returns>
        public LoginData GetLoginData()
        {
            return model.LoginData;
        }

        private void OnAutoLoginCompleted(bool result)
        {
            if (result)
            {
                // �Զ���¼�ɹ��������˺�
                _loginService.SaveLoginData(model.LoginData);
                // ��ʼ������
                LoginOver();
            }
            else
            {
                LogManager.Log($"��¼ʧ��");
                // �ָ���ť����
                model.IsLoginBtnEnabled = true;
                // �Զ���¼ʧ�ܣ��ֶ���¼����ʾ��¼��
                ShowLoginBox(true);
            }
        }

        /// <summary>
        /// ��¼����
        /// </summary>
        private async void LoginOver()
        {
            LogManager.Log($"��¼�ɹ�");
            // ���ص�¼����
            ServiceLocator.Get<IUIManager>().DestroyView(AbKeyCollection.Ui, this);
            // ��ʾ��ʼ����
            //BeginController beginController = await UIManager.Instance.CreateViewAsync<BeginView, BeginModel, BeginController>(E_UILayer.Mid, );
            // ������
            //await beginController.CheckUpdate();
        }

        private async void OnLoginClick()
        {
            // ����У�飨���� Model ������
            if (!model.CheckLoginData())
            {
                LogManager.Log("�˺Ż������ʽ����");
                return;
            }

            // ���õ�¼��ť���޸� Model ���� �� �Զ����� View��
            model.IsLoginBtnEnabled = false;

            // �����ⲿ������������
            await _loginService.LoginAsync(model.LoginData);
        }
    }
}