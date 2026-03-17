using System.Threading.Tasks;
using Core.Loader.Object;
using Core.Log;
using Core.Service;
using HotUpdate.Core.Camera;
using HotUpdate.Core.Module;

namespace HotUpdate.Camera
{
    /// <summary>
    /// 相机模块
    /// </summary>
    public class CameraModule : ICameraModule
    {
        public int Priority => 1;

        public Task InitModuleAsync()
        {
            ServiceLocator.Register<IOrbitCameraGeter>(new OrbitCameraGeter(ServiceLocator.Get<IPrefabLoader>()));
            LogManager.Log($"{nameof(CameraModule)}.{nameof(InitModuleAsync)}:Camera module initialization completed");
            return Task.CompletedTask;
        }
    }
}
