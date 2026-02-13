using System;
using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.Config;
using Core.GlobalEvent;
using Core.Loader.UI;
using Core.Log;
using Core.Service;
using Game.Interact;
using GameHotUpdate.Interact.UI;

namespace GameHotUpdate.Main.UI.Logic
{
    /// <summary>
    /// 交互逻辑处理类
    /// 负责交互相关UI的创建、初始化及数据同步等核心逻辑
    /// 继承自主界面逻辑基类 MainLogic，关联主控制器、主模型、主视图
    /// </summary>
    public class InteractLogic : MainLogic
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mainController">主控制器，用于业务逻辑调度</param>
        /// <param name="mainModel">主数据模型，用于存储交互相关数据</param>
        /// <param name="mainView">主视图，用于交互UI的渲染更新</param>
        public InteractLogic(MainController mainController, MainModel mainModel, MainView mainView) : base(mainController, mainModel, mainView)
        {

        }

        /// <summary>
        /// 逻辑初始化方法
        /// 可在此添加交互逻辑初始化的前置操作（如数据预加载、事件注册等）
        /// </summary>
        public override void Init()
        {
            // 订阅交互事件（当触发InteractEvent时，执行OnInteractEvent回调）
            ServiceLocator.Get<IEventCenter>().SubscribeEvent<InteractEvent>(OnInteractEvent);
        }
        
        /// <summary>
        /// 交互事件回调方法
        /// 执行时机：接收到InteractEvent事件时触发
        /// </summary>
        /// <param name="interactEvent">交互事件数据（包含可交互对象列表）</param>
        private void OnInteractEvent(InteractEvent interactEvent)
        {
            CreateInteract(interactEvent.Interactables);
        }

        /// <summary>
        /// 创建交互UI列表
        /// 根据传入的可交互对象集合，批量创建对应的交互UI并完成初始化
        /// </summary>
        /// <param name="interactables">可交互对象集合（如NPC、道具等可触发交互的对象）</param>
        public async void CreateInteract(List<IInteractable> interactables)
        {
            try
            {
                // 初始化交互UI列表，容量与可交互对象集合一致，减少内存扩容开销
                var interactUIs = new List<InteractUI>(interactables.Count);
                // 遍历可交互对象，为每个对象创建对应的交互UI
                foreach (var interactable in interactables)
                {
                    // 从UI资源包中异步加载交互UI预制体并实例化
                    // EAssetBundleType.UI：指定资源包类型为UI
                    // ResKeyCollection.InteractUI：交互UI的资源标识键
                    var interactUI = await ServiceLocator.Get<IUiLoader>().GetUIObject<InteractUI>(EAssetBundleType.UI, ResKeyCollection.InteractUI, mainView.InteractContent);
                    LogManager.Log($"{nameof(InteractLogic)}.{nameof(CreateInteract)}: {interactUI}");
                    // 初始化交互UI的显示数据（设置发言者/交互对象名称）
                    interactUI.Init(interactable.NpcInfo.f_speakerName);
                    // 将初始化完成的交互UI加入列表
                    interactUIs.Add(interactUI);
                }
            
                // 将创建好的交互UI列表存入主数据模型，供全局业务逻辑调用
                mainModel.CacheInteracts(interactUIs);
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(InteractLogic)}.{nameof(CreateInteract)}: {e.Message}");
            }
        }

        public override void Dispose()
        {
            // 取消交互事件的订阅
            ServiceLocator.Get<IEventCenter>().UnsubscribeEvent<InteractEvent>(OnInteractEvent);
            base.Dispose();
        }
    }
}