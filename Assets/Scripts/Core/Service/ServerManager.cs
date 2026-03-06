using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Log;
using Core.Service.Login;
using Core.Singleton;

namespace Core.Service
{
    /// <summary>
    /// ���������
    /// TODO�������ǹ������еķ��񣬲��ṩҵ���߼���ҵ���߼��о���ķ����߼�ʵ���Լ��Ľӿ�ʵ�֣��������Ҫ�ع�
    /// </summary>
    public class ServerManager : SingletonBase<ServerManager>, IServerManager
    {
        // ����ӿ����͵��������ʵ����ӳ��
        private readonly Dictionary<Type, object> _typeToSeverMap = new Dictionary<Type, object>();
        private int priority;

        private ServerManager()
        {
            _typeToSeverMap.Add(typeof(LoginService), new LoginService());
        }

        /// <summary>
        /// ��ȡ��¼����ʵ��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ILoginService GetService<T>() where T : class, new()
        {
            return _typeToSeverMap[typeof(T)] as ILoginService;
        }

        /// <summary>
        /// �����Զ���¼
        /// </summary>
        /// <returns></returns>
        public async Task TryAutoLogin()
        {
            //LoginController loginController = await UIManager.Instance.CreateViewAsync<LoginView, LoginModel, LoginController>(E_UILayer.Mid);
            //LoginData loginData = loginController.GetLoginData();

            //// У�黺����Ч��
            //if (VerifyValidity(loginData))
            //{
            //    // ִ���Զ���¼��������ͨ��¼�߼���
            //    await (_typeToSeverMap[typeof(LoginService)] as ILoginService).LoginAsync(loginData);
            //}
            //// û�б��ػ���
            //else
            //{
            //    // �Զ���¼ʧ�ܣ���ʾ��¼��
            //    UIManager.Instance.GetView<LoginController>().ShowLoginBox(true);
            //}
            await  Task.CompletedTask;
        }

        /// <summary>
        /// ���ص�¼����
        /// </summary>
        /// <returns></returns>
        public async Task<LoginData> LoadLoginData()
        {
            return await (_typeToSeverMap[typeof(LoginService)] as ILoginService).LoadLoginData();
        }

        /// <summary>
        /// У����Ч��
        /// </summary>
        /// <param name="loginData"></param>
        /// <returns></returns>
        private bool VerifyValidity(LoginData loginData)
        {
            // У�黺����Ч��
            if (string.IsNullOrEmpty(loginData.account) || string.IsNullOrEmpty(loginData.password))
            {
                LogManager.Log("�޻���ĵ�¼��Ϣ");
                return false;
            }
            return true;
        }

        public override int Priority => priority;

        public override Task InitAsync()
        {
            throw new NotImplementedException();
        }
    }
}