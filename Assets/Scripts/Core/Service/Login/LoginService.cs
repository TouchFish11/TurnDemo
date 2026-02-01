using System.Threading.Tasks;
using Core.DataPersistence.Binary;
using Core.Net.FrameSync.Manager;
using Core.Utility;
using UnityEngine.Events;

namespace Core.Service.Login
{
    /// <summary>
    /// ��¼����
    /// </summary>
    public class LoginService : ILoginService
    {
        public event UnityAction<bool> OnAutoLoginCompleted;

        public async Task LoginAsync(LoginData loginData)
        {
            // �����ͻ���
            NetManager.Instance.StartClient("127.0.0.1", 8080);
            //�ȴ����ӳɹ�
            await TaskUtility.WaitUntil(() => NetManager.Instance.GetTcpClient().ConnectData != null);
            // ִ�лص�
            OnAutoLoginCompleted?.Invoke(NetManager.Instance.Connected);
        }

        public void SaveLoginData(LoginData loginData)
        {
            ServiceLocator.Get<IBinaryDataManager>().Save(FileUtility.LocalLoginDataFileName, loginData);
        }

        public LoginData LoadLoginData()
        {
            return ServiceLocator.Get<IBinaryDataManager>().Load<LoginData>(FileUtility.LocalLoginDataFileName);
        }
    }
}
