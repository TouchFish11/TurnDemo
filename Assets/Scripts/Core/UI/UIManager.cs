using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.UI.ViewController;
using UnityEngine;
using Logger = Core.Log.Logger;
using Object = UnityEngine.Object;

namespace Core.UI
{
    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIManager : IUIManager
    {
        // 界面唯一ID
        private static int _panelId;
        // 存储打开的界面
        private readonly Dictionary<int, IPanelInfo> _panels = new();
        // 上层
        private Transform _topLayer;
        // 中层
        private Transform _midLayer;
        // 底层
        private Transform _botLayer;
        // 系统层
        private Transform _systemLayer;
        // 对象生成器
        private readonly ObjectSpawner _objectSpawner;
        // canvas缓存对象
        private GameObject _uiRoot;
        
        private UIManager(ObjectSpawner spawner)
        {
            _objectSpawner = spawner;
        }

        public async Task InitUIManagerAsync(string uiRoot)
        {
            // 获取画布实例
            var uiRootObj = await _objectSpawner.SpawnAsync<GameObject>(uiRoot);
            // 获取画布、UI摄像机实例
            Canvas = uiRootObj.GetComponentInChildren<Canvas>();
            UICamera = uiRootObj.GetComponentInChildren<Camera>();
            
            // 获取对应层级对象位置
            _topLayer = Canvas.transform.Find("Top");
            _midLayer = Canvas.transform.Find("Mid");
            _botLayer = Canvas.transform.Find("Bot");   
            _systemLayer = Canvas.transform.Find("System");
            
            Object.DontDestroyOnLoad(uiRootObj);
            
            // 缓存对象
            _uiRoot = uiRootObj;
        }
        
        public Transform GetLayer(E_UILayer layer)
        {
            return layer switch
            {
                E_UILayer.Top => _topLayer,
                E_UILayer.Mid => _midLayer,
                E_UILayer.Bot => _botLayer,
                E_UILayer.System => _systemLayer,
                _ => null,
            };
        }
        
        public async Task<TController> CreateViewAsync<TView, TController>(
            string panelName, E_UILayer layer, Vector2 pos = default, Quaternion quaternion = default)
            where TView : UIView, IuiView where TController : class, IuiController
        {
            // 初始化控制器
            var controller = DIContainer.Create<TController>();
            try
            {
                // 获取面板
                var viewObj = await _objectSpawner.SpawnAsync<TView>(panelName,GetLayer(layer), pos, quaternion);
                // 生成该界面的唯一ID
                var id = GenerateId();
                await controller.Init(id, viewObj);
                // 初始化面板信息
                var newInfo = new PanelInfo<TView>(id, viewObj, controller);
                // 存储面板信息
                _panels.Add(id, newInfo);
                return controller;
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(UIManager)}.{nameof(CreateViewAsync)}: Create/Init panel error,{e.Message}");
                return controller;
            }
        }
        
        public async Task DestroyView(int panelId)
        {
            if (_panels.TryGetValue(panelId, out var panelInfo))
            {
                // 调用控制器的销毁
                await panelInfo.Controller.Dispose();
                // 回收界面
                _objectSpawner.Release(panelInfo.View, true);
                // 从缓存中移除
                _panels.Remove(panelId);
            }
        }
        
        public async Task SetViewActive(int panelId, bool isActive)
        {
            if (_panels.TryGetValue(panelId, out var panelInfo))
            {
                if (!isActive)
                {
                    await panelInfo.Controller.InActivate();
                }
                else
                {
                    await panelInfo.Controller.Activate();
                }
            }
        }

        public TController GetController<TController>() where TController : IuiController
        {
            foreach (var basePanelInfo in _panels.Values)
            {
                if (basePanelInfo.Controller is TController controller)
                {
                    return controller;
                }
            }
            Logger.LogError($"{nameof(UIManager)}.{nameof(GetController)}: Controller({typeof(TController)}) is not found.");
            return default;
        }
        
        public Task Clear()
        {
            // 回收画布和摄像机
            _objectSpawner.Release(_uiRoot, true);
            Canvas = null;
            UICamera = null;
            
            var list = new List<Task>();
            // 销毁所有界面
            foreach (var id in _panels.Keys)
                list.Add(DestroyView(id));
            
            // 清空缓存
            _panels.Clear();
            _objectSpawner.Dispose();
            return Task.WhenAll(list);
        }

        /// <summary>
        /// 生成界面唯一ID
        /// </summary>
        /// <returns></returns>
        private static int GenerateId()
        {
            return ++_panelId;
        }
        
        public Dictionary<int, IPanelInfo>.ValueCollection Panels => _panels.Values;

        public Canvas Canvas { get; private set; }

        public Camera UICamera { get; private set; }
    }
}
