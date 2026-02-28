using System.Threading.Tasks;
using UnityEngine.Events;

namespace Core.Service.Login
{
    /// <summary>
    /// ��¼����ӿ�
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// �Զ���¼����¼�
        /// </summary>
        public event UnityAction<bool> OnAutoLoginCompleted;

        /// <summary>
        /// �첽��¼
        /// </summary>
        /// <returns></returns>
        Task LoginAsync(LoginData loginData);

        /// <summary>
        /// �����¼����
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        void SaveLoginData(LoginData loginData);

        /// <summary>
        /// ���ص�¼����
        /// </summary>
        Task<LoginData> LoadLoginData();
    }
}
