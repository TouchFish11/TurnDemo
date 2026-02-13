using Core.UI;
using Core.UI.MVC;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.Login.UI
{
    /// <summary>
    /// ��¼����
    /// </summary>
    public class LoginView : UIView
    {
        [Inject] private InputField inputAccount;
        [Inject] private InputField inputPassword;
        [Inject] private Button btnLogin;

        [Inject(1)] private RectTransform loginBox;

        /// <summary>
        /// ���½��棨���� Controller ָ�
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [System.Obsolete]
        public void UpdateView(string key, object value)
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
}
