using System.Threading.Tasks;

namespace HotUpdate.Core.Camera
{
    public interface IOrbitCameraGeter
    {
        public IOrbitCameraController OrbitCameraController { get; set; }

        /// <summary>
        /// 异步创建玩家主相机控制器
        /// 从资源包中加载主相机预制体并初始化相机控制器
        /// </summary>
        /// <returns>初始化完成的轨道相机控制器实例</returns>
        Task<IOrbitCameraController> CreateMainCamera();
    }
}
