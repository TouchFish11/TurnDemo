using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.GlobalEvent;
using Core.GlobalEvent.Events;
using Core.Mono;
using Core.Serialize.Binary;
using HotUpdate.Base.Animation;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Object;
using HotUpdate.Base.UI;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Cameras;
using HotUpdate.Game.Dialogue;
using HotUpdate.Game.Inputs;
using HotUpdate.Game.Interact;
using HotUpdate.Game.Main.FloatingText;
using HotUpdate.Game.Main.Move;

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
        [Inject] private IBinaryDataManager _binaryDataManager;
        
        // Key：实体的索引序号（自增），Value：对应的战斗实体对象ID；用于快速管理和访问挂载到玩家的多个战斗实体
        private readonly Dictionary<int, int> indexToRoleIdMap = new();
        // 字典：玩家UID映射到对应的实体对象，用于快速查找玩家
        private readonly Dictionary<int, IPlayerObject> roleIdToEntityMap = new();
        // 环绕式第三人称相机控制器
        private OrbitCameraController _cameraController;
        
        /// <summary>
        /// 当前控制的实体
        /// </summary>
        public IEntityObject CurrentEntity => roleIdToEntityMap[1];
        
        public PlayerManager(IEventCenter eventCenter)
        {
            eventCenter.SubscribeEvent<OpenViewEvent>(OnOpenViewEvent, OpenViewEventFilter);
            eventCenter.SubscribeEvent<CloseViewEvent>(OnCloseViewEvent, CloseViewEventFilter);
        }

        public async Task CreatePlayer(int id)
        {
            // Test
            const string roleKey = AssetKeys.Prefab_Warrior;
            id = 1;
            var roleInfo = _binaryDataManager.GetConfig<RoleInfoContainer>(EConfigLoadType.Excel).dataDic[id];
            // 从资源包加载战士预制体，并挂载到玩家节点下
            var roleObj = await _objectSpawner.SpawnAsync<PlayerObject>(roleKey);
            roleObj.SetRoleInfo(roleInfo);
            EntityHelper.InitEntity(roleObj);
            AddWorldComponent(roleObj);
            // 初始化玩家相机
            await CreateMainCamera();
            // 设置跟随对象
            _cameraController.SetTarget(roleObj);
            // 设置相机
            roleObj.GetComponent<MoveComponent>().SetCamera(_cameraController);
            // 将玩家对象加入字典管理
            roleIdToEntityMap.Add(id, roleObj);
            _floatingTextManager.SetPlayer(roleObj.transform);
        }

        private static void AddWorldComponent(EntityObject entityObject)
        {
            // 挂载动画控制器组件
            entityObject.AddComponent<AnimatorComponent>();
            // 挂载输入组件：处理玩家的输入事件
            entityObject.AddComponent<InputComponent>();
            // 挂载普通动画组件：处理玩家基础动画状态
            entityObject.AddComponent<NormalAnimationComponent>();
            // 挂载移动组件：处理玩家的位移逻辑（坐标更新、移动速度、碰撞检测等）
            entityObject.AddComponent<MoveComponent>();
            // 挂载交互组件：处理玩家与场景/其他实体的交互逻辑（拾取、对话触发等）
            entityObject.AddComponent<InteractComponent>();
            // 挂载对话组件：处理玩家的对话流程、剧情触发、文本展示等逻辑
            entityObject.AddComponent<DialogueComponent>();
        }
        
        /// <summary>
        /// 设置玩家的默认战斗实体
        /// 用于初始化玩家默认显示/控制的实体（如初始角色）
        /// </summary>
        private void SetDefault()
        {

        }
        
        /// <summary>
        /// 清理所有玩家对象
        /// </summary>
        public void Clear()
        {
            // 遍历所有玩家实体，执行销毁逻辑
            foreach (var entity in roleIdToEntityMap.Values)
            {
                entity.Destroy(); // 执行实体内部销毁逻辑
                EngineUtility.Destroy(entity.GameObject); // 销毁GameObject对象
            }

            // 清空字典，释放引用
            roleIdToEntityMap.Clear();
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
            CurrentEntity.GetComponent<InputComponent>().DisableInput();
            CurrentEntity.GetComponent<NormalAnimationComponent>().Play(EAnimationType.Idle);
            CurrentEntity.GetComponent<MoveComponent>().Disable();
        }

        private bool OpenViewEventFilter(OpenViewEvent openViewEvent)
        {
            return openViewEvent.UIController is IBlockOperation blockOperation && blockOperation.BlockOperation && roleIdToEntityMap.ContainsKey(1);
        }
        
        /// <summary>
        /// UI界面打开事件回调
        /// </summary>
        /// <param name="closeViewEvent"></param>
        private void OnCloseViewEvent(CloseViewEvent closeViewEvent)
        {
            CurrentEntity.GetComponent<InputComponent>().EnableInput();
            CurrentEntity.GetComponent<MoveComponent>().Enable();
        }

        private bool CloseViewEventFilter(CloseViewEvent closeViewEvent)
        {
            return closeViewEvent.UIController is IBlockOperation blockOperation && blockOperation.BlockOperation && roleIdToEntityMap.ContainsKey(1001);
        }
    }
}