using System.Threading.Tasks;
using Core.Loader.Object;
using Core.Service;
using HotUpdate.Core.Camera;
using HotUpdate.Core.Module;

namespace HotUpdate.Camera
{
    /// <summary>
    /// 相机模块
    /// </summary>
    public class CameraModule : IModule
    {
        public Task InitModuleAsync()
        {
            ServiceLocator.Register<IOrbitCameraGeter>(new OrbitCameraGeter(ServiceLocator.Get<IPrefabLoader>()));
            
            return Task.CompletedTask;
        }
    }
}
