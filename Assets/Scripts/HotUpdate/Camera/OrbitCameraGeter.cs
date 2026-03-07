using System.Threading.Tasks;
using Core.Loader.Object;
using Core.Service;
using HotUpdate.Common;
using HotUpdate.Core.Camera;

namespace HotUpdate.Camera
{
    /// <summary>
    /// 相机获取器
    /// </summary>
    public class OrbitCameraGeter : IOrbitCameraGeter
    {
        public IOrbitCameraController OrbitCameraController { get; set; }
        
        /// <summary>
        /// 异步创建玩家主相机控制器
        /// 从资源包中加载主相机预制体并初始化相机控制器
        /// </summary>
        /// <returns>初始化完成的轨道相机控制器实例</returns>
        public async Task<IOrbitCameraController> CreateMainCamera()
        {
            OrbitCameraController ??= await ServiceLocator.Get<IPrefabLoader>()
                .GetObjectAsync<OrbitCameraController>(AbKeyCollection.Camera, ResKeyCollection.MainCamera, null);
            return OrbitCameraController;
        }
    }
}
