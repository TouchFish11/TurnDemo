using System.Threading.Tasks;
using Core.Loader.Object;
using Core.Mono;
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
        private readonly IPrefabLoader _prefabLoader;
        
        public IOrbitCameraController OrbitCameraController { get; set; }

        public OrbitCameraGeter(IPrefabLoader prefabLoader)
        {
            _prefabLoader = prefabLoader;
        }
        
        public async Task<IOrbitCameraController> CreateMainCamera()
        {
            OrbitCameraController ??= await _prefabLoader.GetObjectAsync<OrbitCameraController>(
                AbKeyCollection.Camera, 
                ResKeyCollection.MainCamera, 
                null);
            return OrbitCameraController;
        }

        public void DestroyMainCamera()
        {
            EngineUtility.Destroy(OrbitCameraController.Transform.gameObject);
            OrbitCameraController = null;
        }
    }
}
