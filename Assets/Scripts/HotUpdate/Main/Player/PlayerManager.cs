using System.Collections.Generic;
using Core.Components;
using Core.GlobalEvent;
using Core.GlobalEvent.Events;
using Core.Loader.Object;
using Core.Service;
using Core.Singleton;
using HotUpdate.Common;
using HotUpdate.Core.Animation;
using HotUpdate.Core.Camera;
using HotUpdate.Core.Input;
using HotUpdate.Core.Main;
using HotUpdate.Core.Module;
using HotUpdate.Core.MVC;
using HotUpdate.Main.Global.UI;
using HotUpdate.Main.Move;
using UnityEngine;

namespace HotUpdate.Main.Player
{
    using Task = System.Threading.Tasks.Task;
    
    /// <summary>
    /// 玩家管理器
    /// 负责玩家对象的创建、管理、销毁等核心逻辑
    /// </summary>
    public class PlayerManager : IPlayerManager
    {
        private readonly IPrefabLoader _prefabLoader;

        // 字典：玩家UID映射到对应的实体对象，用于快速查找玩家
        private readonly Dictionary<uint, IEntityObject> uidToEntityMap = new();

        private const string DefaultPlayerName = "Player";
        // 主玩家对象（固定UID为1001）
        public IEntityObject MainPlayer => uidToEntityMap[1001];
        
        public PlayerManager(IPrefabLoader prefabLoader,  IEventCenter eventCenter)
        {
            _prefabLoader = prefabLoader;
            eventCenter.SubscribeEvent<OpenViewEvent>(OnOpenViewEvent, OpenViewEventFilter);
            eventCenter.SubscribeEvent<CloseViewEvent>(OnCloseViewEvent, OpenViewEventFilter);
        }

        /// <summary>
        /// 创建玩家对象
        /// </summary>
        /// <param name="uid">玩家唯一标识</param>
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
            var warrior = await _prefabLoader.GetGameObjectAsync(AbKeyCollection.Prefab, ResKeyCollection.Prefab_Main_Warrior, main.transform);
            // 给战士预制体添加战士逻辑组件，并关联到主玩家
            ServiceLocator.Get<IModuleManager>().GetModule<IBattleModule>().AddWarrior(warrior);
            // 初始化主玩家基础数据（参数1为示例配置ID）
            main.BaseInit(1);
            // 初始化玩家相机
            var camera = await ServiceLocator.Get<IOrbitCameraGeter>().CreateMainCamera();
            // 设置跟随对象
            camera.SetTarget(main);
            // 设置相机
            main.InitCamera(camera);
            // 将玩家对象加入字典管理
            uidToEntityMap.Add(uid, main);
            ServiceLocator.Get<IFloatingTextManager>().SetPlayer(main.transform);
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
                Object.Destroy(entity.GameObject); // 销毁GameObject对象
            }

            // 清空字典，释放引用
            uidToEntityMap.Clear();
            // 销毁主摄像机
            ServiceLocator.Get<IOrbitCameraGeter>().DestroyMainCamera();
        }

        /// <summary>
        /// UI界面打开事件回调
        /// </summary>
        /// <param name="openViewEvent"></param>
        private void OnOpenViewEvent(OpenViewEvent openViewEvent)
        {
            MainPlayer.GetComponent<IInputComponent>().DisEnableInput();
            MainPlayer.GetComponent<INormalAnimationComponent>().SetAnimationState((int)E_AnimationType.Idle);
            MainPlayer.GetComponent<MoveComponent>().Disable();
        }

        private bool OpenViewEventFilter(OpenViewEvent openViewEvent)
        {
            return openViewEvent.UIController is not IMainController &&
                   openViewEvent.UIController is not IDialogueController &&
                   openViewEvent.UIController is not GlobalMessageController && 
                   uidToEntityMap.ContainsKey(1001);
        }
        
        /// <summary>
        /// UI界面打开事件回调
        /// </summary>
        /// <param name="closeViewEvent"></param>
        private void OnCloseViewEvent(CloseViewEvent closeViewEvent)
        {
            MainPlayer.GetComponent<IInputComponent>().EnableInput();
            MainPlayer.GetComponent<MoveComponent>().Enable();
        }

        private bool OpenViewEventFilter(CloseViewEvent closeViewEvent)
        {
            return closeViewEvent.UIController is not IMainController &&
                   closeViewEvent.UIController is not IDialogueController &&
                   closeViewEvent.UIController is not GlobalMessageController &&
                   uidToEntityMap.ContainsKey(1001);
        }
    }
}