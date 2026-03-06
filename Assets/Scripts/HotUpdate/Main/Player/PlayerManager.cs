using System.Collections.Generic;
using Core.Components;
using Core.GlobalEvent;
using Core.GlobalEvent.Events;
using Core.Loader.Object;
using Core.Service;
using Core.Singleton;
using HotUpdate.Animation;
using HotUpdate.Animation.Component;
using HotUpdate.Battle.Object.Role.Warrior;
using HotUpdate.Config;
using HotUpdate.Core.Main;
using HotUpdate.Dialogue.UI;
using HotUpdate.Main.FloatingText;
using HotUpdate.Main.Move;
using HotUpdate.Main.Player;
using HotUpdate.Main.UI;
using UnityEngine;

namespace HotUpdate.Main
{
    using Task = System.Threading.Tasks.Task;
    
    /// <summary>
    /// 玩家管理器
    /// 负责玩家对象的创建、管理、销毁等核心逻辑
    /// </summary>
    public class PlayerManager : SingletonBase<PlayerManager>, IPlayerManager
    {
        public override int Priority => -1;

        private readonly IPrefabLoader _prefabLoader = ServiceLocator.Get<IPrefabLoader>();
        private readonly IEventCenter _eventCenter = ServiceLocator.Get<IEventCenter>();
        // 字典：玩家UID映射到对应的实体对象，用于快速查找玩家
        private readonly Dictionary<uint, IEntityObject> uidToEntityMap = new();

        private const string DefaultPlayerName = "Player";
        // 主玩家对象（固定UID为1001）
        public IEntityObject MainPlayer => uidToEntityMap[1001];
        
        private PlayerManager()
        {

        }

        public override Task InitAsync()
        {
            _eventCenter.SubscribeEvent<OpenViewEvent>(OnOpenViewEvent, OpenViewEventFilter);
            _eventCenter.SubscribeEvent<CloseViewEvent>(OnCloseViewEvent, OpenViewEventFilter);
            return Task.CompletedTask;
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

            // 添加角色控制器组件（用于移动、碰撞等物理交互）
            var characterController = mainObj.AddComponent<CharacterController>();
            characterController.center = new Vector3(0, 1, 0); // 设置控制器中心偏移

            // 添加主玩家核心逻辑组件
            var main = mainObj.AddComponent<MainPlayer>();
            
            // 从资源包加载战士预制体，并挂载到玩家节点下
            var warrior = await _prefabLoader.GetGameObjectAsync(AbKeyCollection.Prefab, ResKeyCollection.Prefab_Main_Warrior, main.transform);
            // 给战士预制体添加战士逻辑组件，并关联到主玩家
            main.AddEntity(warrior.AddComponent<Warrior>());
            
            // 初始化玩家相机（异步创建主相机控制器）
            await CreateMainCamera();
            // 初始化主玩家基础数据（参数1为示例配置ID）
            main.BaseInit(1);
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
                UnityEngine.Object.Destroy(entity.GameObject); // 销毁GameObject对象
            }

            // 清空字典，释放引用
            uidToEntityMap.Clear();
        }

        /// <summary>
        /// UI界面打开事件回调
        /// </summary>
        /// <param name="openViewEvent"></param>
        private void OnOpenViewEvent(OpenViewEvent openViewEvent)
        {
            MainPlayer.GetComponent<InputComponent>().DisEnableInput();
            MainPlayer.GetComponent<NormalAnimationComponent>().SetAnimationState(E_AnimationType.Idle);
            MainPlayer.GetComponent<MoveComponent>().Disable();
        }

        private bool OpenViewEventFilter(OpenViewEvent openViewEvent)
        {
            return openViewEvent.UIController is not MainController &&
                   openViewEvent.UIController is not DialogueController && uidToEntityMap.ContainsKey(1001);
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

        private bool OpenViewEventFilter(CloseViewEvent closeViewEvent)
        {
            return closeViewEvent.UIController is not MainController &&
                   closeViewEvent.UIController is not DialogueController && uidToEntityMap.ContainsKey(1001);
        }
        
        /// <summary>
        /// 异步创建玩家主相机控制器
        /// 从资源包中加载主相机预制体并初始化相机控制器
        /// </summary>
        /// <returns>初始化完成的轨道相机控制器实例</returns>
        private Task CreateMainCamera()
        {
            // 通过对象构建器从指定资源包加载主相机
            return _prefabLoader.GetObjectAsync<OrbitCameraController>(AbKeyCollection.Camera, ResKeyCollection.MainCamera,
                    null);
        }
    }
}