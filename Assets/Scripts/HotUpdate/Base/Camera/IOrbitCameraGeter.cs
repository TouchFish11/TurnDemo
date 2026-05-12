using System.Threading.Tasks;

namespace HotUpdate.Base.Camera
{
    public interface IOrbitCameraGeter
    {
        /// <summary>
        /// 玩家主相机
        /// </summary>
        public IOrbitCameraController OrbitCameraController { get; set; }

        /// <summary>
        /// 异步创建玩家主相机控制器
        /// </summary>
        /// <returns>初始化完成的轨道相机控制器实例</returns>
        Task<IOrbitCameraController> CreateMainCamera();

        /// <summary>
        /// 销毁主摄像机
        /// </summary>
        void DestroyMainCamera();
    }
}
