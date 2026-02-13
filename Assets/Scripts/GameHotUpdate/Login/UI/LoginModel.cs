using Core.Service.Login;
using Core.UI.MVC;

namespace GameHotUpdate.Login.UI
{
    /// <summary>
    /// ��¼��������
    /// </summary>
    public class LoginModel : UIModel
    {
        // ��¼����
        private LoginData loginData;
        // ��¼��ť�Ƿ�����
        private bool isLoginBtnEnabled = true;
        // ��¼���Ƿ񼤻�
        private bool isActiveLoginBox = false;

        /// <summary>
        /// ��С���볤��
        /// </summary>
        private const int MinPasswordLength = 6;

        public LoginData LoginData
        {
            get => loginData;
            set
            {
                loginData = value;
                // TriggerDataChanged(nameof(loginData), value);
            }
        }

        /// <summary>
        /// ��¼��ť�Ƿ�����
        /// </summary>
        public bool IsLoginBtnEnabled
        {
            get => isLoginBtnEnabled;
            set
            {
                isLoginBtnEnabled = value;
                // TriggerDataChanged(nameof(isLoginBtnEnabled), value);
            }
        }

        public bool IsActiveLoginBox
        {
            get => isActiveLoginBox;
            set
            {
                isActiveLoginBox = value;
                // TriggerDataChanged(nameof(isActiveLoginBox), value);
            }
        }

        /// <summary>
        /// �����˺�
        /// </summary>
        /// <param name="account"></param>
        public void SetAccount(string account)
        {
            loginData.account = account;
            // TriggerDataChanged(nameof(loginData.account), account);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="password"></param>
        public void SetPassword(string password)
        {
            loginData.password = password;
            // TriggerDataChanged(nameof(loginData.password), password);
        }

        /// <summary>
        /// У���¼���ݣ�Model ֻ������У�飬������������
        /// </summary>
        /// <returns></returns>
        public bool CheckLoginData()
        {
            return !string.IsNullOrEmpty(loginData.account) && loginData.password.Length >= MinPasswordLength;
        }
    }
}
