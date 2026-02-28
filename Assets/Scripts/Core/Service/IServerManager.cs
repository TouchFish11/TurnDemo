using System.Threading.Tasks;
using Core.Service.Login;

namespace Core.Service
{
    /// <summary>
    /// ����������ӿ�
    /// </summary>
    public interface IServerManager
    {
        ILoginService GetService<T>() where T : class, new();
        Task<LoginData> LoadLoginData();
        Task TryAutoLogin();
    }
}
