using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.GlobalEvent;
using Core.GlobalEvent.Events;
using Core.Mono;
using HotUpdate.Base.Enums;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Object;
using HotUpdate.Base.UI;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Object.Role.Warrior;
using HotUpdate.Game.Cameras;
using HotUpdate.Game.Inputs;
using HotUpdate.Game.Main.FloatingText;
using HotUpdate.Game.Main.Move;
using UnityEngine;

namespace HotUpdate.Game.Main.Player
{
    /// <summary>
    /// 玩家管理器
    /// 负责玩家对象的创建、管理、销毁等核心逻辑
    /// </summary>
    public class PlayerManager : IPlayerManager
    {
        [Inject] private IFloatingTextManager _floatingTextManager;
        [Inject] private ObjectSpawner _objectSpawner;
        
        // 环绕式第三人称相机控制器
        private OrbitCameraController _cameraController;

        // 字典：玩家UID映射到对应的实体对象，用于快速查找玩家
        private readonly Dictionary<uint, IEntityObject> uidToEntityMap = new();

        /// <summary>
        /// 默认玩家名称
        /// </summary>
        private const string DefaultPlayerName = "Player";
        
        public IEntityObject MainPlayer => uidToEntityMap[1001];
        
        public PlayerManager(IEventCenter eventCenter)
        {
            eventCenter.SubscribeEvent<OpenViewEvent>(OnOpenViewEvent, OpenViewEventFilter);
            eventCenter.SubscribeEvent<CloseViewEvent>(OnCloseViewEvent, CloseViewEventFilter);
        }
        
        public async Task CreatePlayer(uint uid)
        {
            // 创建玩家根节点GameObject
            var mainObj = new GameObject(DefaultPlayerName);
            // 设置玩家初始位置和旋转角度
            mainObj.transform.SetPositionAndRotation(new Vector3(0, 0, -5.6f), Quaternion.identity);

            // 添加角色控制器组件
            var characterController = mainObj.AddComponent<CharacterController>();
            characterController.center = new Vector3(0, 1, 0); // 设置控制器中心偏移

            // 添加主玩家核心逻辑组件
            var main = mainObj.AddComponent<MainPlayer>();
            // 从资源包加载战士预制体，并挂载到玩家节点下
            var warriorObj = await _objectSpawner.SpawnAsync<GameObject>(AssetKeys.Prefab_Main_Warrior, main.transform);
            // 给战士预制体添加战士逻辑组件，并关联到主玩家
            warriorObj.AddComponent<Warrior>();
            // 初始化主玩家基础数据
            main = EntityHelper.InitEntity(main);
            // 初始化玩家相机
            await CreateMainCamera();
            // 设置跟随对象
            _cameraController.SetTarget(main);
            // 设置相机
            main.InitCamera(_cameraController);
            // 将玩家对象加入字典管理
            uidToEntityMap.Add(uid, main);
            _floatingTextManager.SetPlayer(main.transform);
        }

        /// <summary>
        /// 清理所有玩家对象
        /// </summary>
        public void Clear()
        {
            // 遍历所有玩家实体，执行销毁逻辑
            foreach (var entity in uidToEntityMap.Values)
            {
                entity.Destroy(); // 执行实体内部销毁逻辑
                EngineUtility.Destroy(entity.GameObject); // 销毁GameObject对象
            }

            // 清空字典，释放引用
            uidToEntityMap.Clear();
            // 销毁主摄像机
            EngineUtility.Destroy(_cameraController.gameObject);
            _cameraController = null;
        }

        /// <summary>
        /// 创建玩家主相机
        /// </summary>
        private async Task CreateMainCamera()
        {
            var poolObject = await _objectSpawner.SpawnAsync<OrbitCameraController>(AssetKeys.MainCamera);
            _cameraController = poolObject;
        }

        /// <summary>
        /// UI界面打开事件回调
        /// </summary>
        /// <param name="openViewEvent"></param>
        private void OnOpenViewEvent(OpenViewEvent openViewEvent)
        {
            MainPlayer.GetComponent<InputComponent>().DisableInput();
            MainPlayer.GetComponent<NormalAnimationComponent>().SetAnimationState((int)E_AnimationType.Idle);
            MainPlayer.GetComponent<MoveComponent>().Disable();
        }

        private bool OpenViewEventFilter(OpenViewEvent openViewEvent)
        {
            return openViewEvent.UIController is IBlockOperation blockOperation && blockOperation.BlockOperation && uidToEntityMap.ContainsKey(1001);
        }
        
        /// <summary>
        /// UI界面打开事件回调
        /// </summary>
        /// <param name="closeViewEvent"></param>
        private void OnCloseViewEvent(CloseViewEvent closeViewEvent)
        {
            MainPlayer.GetComponent<InputComponent>().EnableInput();
            MainPlayer.GetComponent<MoveComponent>().Enable();
        }

        private bool CloseViewEventFilter(CloseViewEvent closeViewEvent)
        {
            return closeViewEvent.UIController is IBlockOperation blockOperation && blockOperation.BlockOperation && uidToEntityMap.ContainsKey(1001);
        }
    }
}