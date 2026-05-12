using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Mono;
using HotUpdate.Base.Camera;
using HotUpdate.Common;

namespace HotUpdate.Game.Cameras
{
    /// <summary>
    /// 相机获取器
    /// </summary>
    public class OrbitCameraGeter : IOrbitCameraGeter
    {
        [Inject] private ObjectSpawner _objectSpawner;
        
        public IOrbitCameraController OrbitCameraController { get; set; }
        
        public async Task<IOrbitCameraController> CreateMainCamera()
        {
            if (OrbitCameraController != null) 
                return OrbitCameraController;
            
            var poolObject = await _objectSpawner.SpawnAsync<OrbitCameraController>(ResKeyCollection.MainCamera);
            OrbitCameraController = poolObject.Obj;
            return OrbitCameraController;
        }

        public void DestroyMainCamera()
        {
            EngineUtility.Destroy(OrbitCameraController.Transform.gameObject);
            OrbitCameraController = null;
        }
    }
}
