using System;

namespace Core.Service.Login
{
    /// <summary>
    /// ��¼����
    /// </summary>
    [Serializable]
    public struct LoginData
    {
        // �˺�
        public string account;
        // ����
        public string password;

        public LoginData(string account, string password)
        {
            this.account = account;
            this.password = password;
        }
    }
}
