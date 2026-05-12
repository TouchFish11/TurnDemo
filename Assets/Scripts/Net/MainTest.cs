using Core.DI;
using kcp2k;
using Net.Sync;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace Net
{
    public class MainTest : MonoBehaviour
    {
        private void Start()
        {
            Application.runInBackground = true;
            // 注册框架单例
            //DIContainer.RegisterSingletons();
            // 注入依赖
            //DIContainer.InjectDependencies();
            
            DIContainer.BindSingleton<INetGameProxy, NetGameProxy>();
            
            // -----------------
            var config = new NetConfig
            {
                ServerIp = "127.0.0.1",
                ServerPort = 8080,
                Serializer = MessageSerializerGetter.BinaryMessageSerializer(),
                ClientType = EClientType.Kcp,
                KcpConfig = new KcpConfig(DualMode:false, Timeout: 30000)
            };
            
            var proxy = DIContainer.GetInstance<INetGameProxy>().Init(config);
            proxy.OnGameConnected += OnOnGameConnected;
            proxy.Connect();
        }

        private static void OnOnGameConnected(int clientId)
        {
            Logger.Log($"[Net Connect] 已初始化客户端ID:{clientId}");
        }

        private void OnDisable()
        {
            DIContainer.GetInstance<INetGameProxy>().Disconnect();
        }
    }
}
