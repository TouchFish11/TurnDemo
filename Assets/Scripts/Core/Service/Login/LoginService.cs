using System.Threading.Tasks;
using Core.Net.FrameSync.Manager;
using Core.Serialize.Binary;
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
            ServiceLocator.Get<IBinaryDataManager>().SaveAsync(FileUtility.LocalLoginDataFileName, loginData);
        }

        public async Task<LoginData> LoadLoginData()
        {
            return await ServiceLocator.Get<IBinaryDataManager>().LoadAsync<LoginData>(FileUtility.LocalLoginDataFileName);
        }
    }
}
