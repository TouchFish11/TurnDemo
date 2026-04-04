using System;
using System.Collections.Generic;
using Core.Collection;
using Core.GlobalEvent;
using Core.Loader.Object;
using Core.Log;
using Core.Service;
using HotUpdate.Common;
using HotUpdate.Core.Interact;

namespace HotUpdate.Main.UI.Logic
{
    /// <summary>
    /// 交互逻辑处理类
    /// 负责交互相关UI的创建、初始化及数据同步等核心逻辑
    /// 继承自主界面逻辑基类 MainLogic，关联主控制器、主模型、主视图
    /// </summary>
    public class InteractLogic : MainLogic
    {
        private readonly IEventCenter _eventCenter = ServiceLocator.Get<IEventCenter>();
        private readonly IPrefabLoader _prefabLoader = ServiceLocator.Get<IPrefabLoader>();
        
        protected override void OnInit()
        {
            // 订阅交互事件（当触发InteractEvent时，执行OnInteractEvent回调）
            _eventCenter.SubscribeEvent<InteractEvent>(OnInteractEvent);
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
                var uniList = ListUtility.GetUniList<IInteractUI>();
                // 遍历可交互对象，为每个对象创建对应的交互UI
                foreach (var interactable in interactables)
                {
                    // 从UI资源包中异步加载交互UI预制体并实例化
                    var interactUI = await _prefabLoader.GetObjectAsync<IInteractUI>(AbKeyCollection.Ui, ResKeyCollection.InteractUI, mainView.InteractContent);
                    // 初始化交互UI的显示数据（设置发言者/交互对象名称）
                    interactUI.Init(interactable.NpcInfo.f_speakerName);
                    // 将初始化完成的交互UI加入列表
                    uniList.List.Add(interactUI);
                }
            
                // 将创建好的交互UI列表存入主数据模型，供全局业务逻辑调用
                mainModel.CacheInteracts(uniList.List);
                ListUtility.CollectUniList(uniList);
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(InteractLogic)}.{nameof(CreateInteract)}: {e.Message}，{e.StackTrace}");
            }
        }

        public override void ResetData()
        {
            // 取消交互事件的订阅
            _eventCenter.UnsubscribeEvent<InteractEvent>(OnInteractEvent);
            base.ResetData();
        }
    }
}