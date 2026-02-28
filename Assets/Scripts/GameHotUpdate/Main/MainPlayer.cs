using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Service;
using Game.Battle.Objects;
using GameHotUpdate.Animation;
using GameHotUpdate.Cameras;
using GameHotUpdate.Config;
using GameHotUpdate.Dialogue;
using GameHotUpdate.Input;
using GameHotUpdate.Interact;
using GameHotUpdate.Move;
using GameHotUpdate.Object;

namespace GameHotUpdate.Main
{
    /// <summary>
    /// 主玩家核心逻辑类
    /// 负责玩家实体的初始化、组件挂载、战斗实体管理、相机创建等核心逻辑
    /// 继承自 EntityObject，作为游戏内可交互的实体基类
    /// </summary>
    public class MainPlayer : EntityObject
    {
        // 战斗实体对象的索引映射字典
        // Key：实体的索引序号（自增），Value：对应的战斗实体对象接口实例
        // 用于快速管理和访问挂载到玩家的多个战斗实体
        private readonly Dictionary<int, IBattleEntityObject> indexToEntityMap = new(); 

        /// <summary>
        /// 玩家实体基础初始化方法
        /// 重写自 EntityObject 基类，在实体创建时调用
        /// </summary>
        /// <param name="id">玩家实体的唯一标识ID</param>
        public override async void BaseInit(int id)
        {
            // 初始化玩家相机（异步创建主相机控制器）
            await CreateCamera();
            
            // 挂载输入组件：处理玩家的输入事件（键鼠、手柄等）
            AddComponent<InputComponent>();
            
            // 挂载普通动画组件：处理玩家基础动画状态（待机、移动、攻击等）
            AddComponent<NormalAnimationComponent>();
            
            // 挂载移动组件：处理玩家的位移逻辑（坐标更新、移动速度、碰撞检测等）
            AddComponent<MoveComponent>();
            
            // 挂载交互组件：处理玩家与场景/其他实体的交互逻辑（拾取、对话触发等）
            AddComponent<InteractComponent>();
            
            // 挂载对话组件：处理玩家的对话流程、剧情触发、文本展示等逻辑
            AddComponent<DialogueComponent>();
        }

        /// <summary>
        /// 添加战斗实体到玩家的管理列表
        /// </summary>
        /// <param name="entityObject">待添加的战斗实体对象（实现IBattleEntityObject接口）</param>
        public void AddEntity(IBattleEntityObject entityObject)
        {
            // 以当前字典长度作为索引（自增），添加实体到映射表
            indexToEntityMap.Add(indexToEntityMap.Count, entityObject);
            
            // 【注】此处省略实体添加后的其他逻辑（如实体初始化、事件注册等）
            
            // 设置默认战斗实体（如默认角色模型、默认行为等）
            SetDefault();
        }

        /// <summary>
        /// 设置玩家的默认战斗实体
        /// 用于初始化玩家默认显示/控制的实体（如初始角色、默认武器等）
        /// </summary>
        private void SetDefault()
        {
            // 【注】当前逻辑注释待启用：获取索引为0的默认实体，绑定其动画控制器到玩家动画组件
            //var defaultEntity = indexToEntityMap[0];
            //GetComponent<NormalAnimationComponent>().SetAnimator(defaultEntity.GetComponentInChildren<AnimatorComponent>().Animator);
        }

        /// <summary>
        /// 异步创建玩家主相机控制器
        /// 从资源包中加载主相机预制体并初始化相机控制器
        /// </summary>
        /// <returns>初始化完成的轨道相机控制器实例</returns>
        private async Task<OrbitCameraController> CreateCamera()
        {
            // 通过对象构建器从指定资源包加载主相机
                var hotfixOrbitCameraController = await ServiceLocator.Get<IObjectBuilder>()
                .GetHotfixObject<OrbitCameraController>(AbKeyCollection.Camera, ResKeyCollection.MainCamera,
                    null);
                return hotfixOrbitCameraController;
        }
    }
}