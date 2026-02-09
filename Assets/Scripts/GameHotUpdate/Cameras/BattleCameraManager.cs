using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Config;
using Core.Pool;
using Core.Service;
using Core.Singleton;
using Game.Objects;
using UnityEngine;

namespace GameHotUpdate.Cameras
{
    /// <summary>
    /// 战斗相机管理器
    /// </summary>
    public class BattleCameraManager : SingletonBase<BattleCameraManager>, IBattleCameraManager
    {
        // 当前激活的相机
        private Camera _currentActiveCamera;
        
        private BattleCameraManager()
        {

        }
        
        public async Task<Camera> CreateCamera(Transform cameraTrans, Vector3 localPos, Quaternion localRot)
        {
            if(_currentActiveCamera)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(_currentActiveCamera.gameObject);
                _currentActiveCamera = null;
            }
            
            var cameraObj = await ServiceLocator.Get<IObjectBuilder>()
                .GetGameobject(EAssetBundleType.Camera, ResKeyCollection.BattleCamera, cameraTrans);
            _currentActiveCamera = cameraObj.GetComponent<Camera>();
            _currentActiveCamera.transform.SetLocalPositionAndRotation(localPos, localRot);
            
            return _currentActiveCamera;
        }

        public async Task<Camera> CreateCamera(Transform cameraTrans, Vector3 localPos, Quaternion localRot, int mask)
        {
            if(_currentActiveCamera)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(_currentActiveCamera.gameObject);
                _currentActiveCamera = null;
            }
            
            var cameraObj = await ServiceLocator.Get<IObjectBuilder>()
                .GetGameobject(EAssetBundleType.Camera, ResKeyCollection.BattleCamera, cameraTrans);
            _currentActiveCamera = cameraObj.GetComponent<Camera>();
            _currentActiveCamera.transform.SetLocalPositionAndRotation(localPos, localRot);

            SetMask(_currentActiveCamera, mask);
            
            return _currentActiveCamera;
        }

        private static void SetMask(Camera camera, int mask)
        {
            camera.cullingMask = 0;
            // 设置遮罩
            camera.cullingMask = mask;
        }
    }
}
