using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Loader.Object;
using Core.Log;
using Core.Service;
using Core.Singleton;
using Core.UI.MVC;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.UI
{
    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIManager : SingletonBase<UIManager>, IUIManager
    {
        public override int Priority => 2;
        // 存储打开的界面
        private readonly List<IPanelInfo> _panels = new();
        // 上层
        private Transform _topLayer;
        // 中层
        private Transform _midLayer;
        // 底层
        private Transform _botLayer;
        // 系统层
        private Transform _systemLayer;
        // 预制体加载器对象
        private IPrefabLoader _prefabLoader;
        
        private UIManager()
        {
        }

        public override Task InitAsync()
        {
            // 要先初始化工厂才能拿到加载器实例
            _prefabLoader = ServiceLocator.Get<IPrefabLoader>();
            return Task.CompletedTask;
        }

        public async Task InitUIManagerAsync(string defaultAbName, string canvasName, string uiCameraName)
        {
#if EDITOR_TEST_AB || !UNITY_EDITOR
            // 创建画布实例
            Canvas = await ServiceLocator.Get<IPrefabLoader>().GetObjectAsync<Canvas>(defaultAbName, canvasName, null);
            Object.DontDestroyOnLoad(Canvas.gameObject);

            // 获取对应层级对象位置
            _topLayer = Canvas.transform.Find("Top");
            _midLayer = Canvas.transform.Find("Mid");
            _botLayer = Canvas.transform.Find("Bot");   
            _systemLayer = Canvas.transform.Find("System");
            
            // 创建UI相机实例
            UICamera = await ServiceLocator.Get<IPrefabLoader>().GetObjectAsync<Camera>(defaultAbName, uiCameraName, null);
            Object.DontDestroyOnLoad(UICamera.gameObject);
            // 设置UI摄像机
            Canvas.worldCamera = UICamera;
#else
            //加载画布资源
            var canvasObj = EditorResManager.Instance.LoadEditorAsset<GameObject>(ResKeyCollection.Canvas);
            //实例化画布对象
            var canvasInstance = Object.Instantiate(canvasObj);
            //记录画布对象
            Canvas = canvasInstance.GetComponent<Canvas>();
            //过场景不移除
            Object.DontDestroyOnLoad(canvasInstance);
            //获取对应层级对象位置
            _topLayer = Canvas.transform.Find("Top");
            _midLayer = Canvas.transform.Find("Mid");
            _botLayer = Canvas.transform.Find("Bot");
            _systemLayer = Canvas.transform.Find("System");
            //加载UI摄像机资源
            var uiCameraObj = EditorResManager.Instance.LoadEditorAsset<GameObject>(ResKeyCollection.UICamera);
            //实例化摄像机对象
            var uiCameraInstance = Object.Instantiate(uiCameraObj);
            //记录UI摄像机
            UICamera = uiCameraInstance.GetComponent<Camera>();
            //过场景不移除
            Object.DontDestroyOnLoad(uiCameraInstance);
            //设置UI摄像机
            Canvas.worldCamera = UICamera;
            await Task.CompletedTask;
#endif
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
        
        public async Task<TController> CreateViewAsync<TView, TModel, TController>(string abName, E_UILayer layer, string panelName)
            where TView : UIBehaviourBase, IuiView where TModel : IuiModel, new() where TController : class, IuiController, new()
        {
#if EDITOR_TEST_AB || !UNITY_EDITOR
            // 获取面板
            var view = await ServiceLocator.Get<IPrefabLoader>().GetObjectAsync<TView>(abName, panelName, GetLayer(layer));
            // 初始化控制器
            var controller = new TController();
            var model = new TModel();
            await controller.Init(view, model);
            await controller.Show();
            // 初始化面板信息
            var newInfo = new PanelInfo<TView, TModel, TController>(view, model, controller);
            // 存储面板信息
            _panels.Add(newInfo);
            return controller;
#else
            //自定义存储名称
            var assetName = $"{typeof(TView).Name}";
            // 不存在工厂
            if (!_typeToCtrlFactoryMap.TryGetValue(typeof(TController), out var iFactory))
            {
                LogManager.LogWarning($"未初始化{typeof(TController)}控制器工厂");
                return null;
            }

            // 存在工厂
            var factory = iFactory as UIControllerFactory<TView, TModel, TController>;
            // 获取面板
            var view = await ObjectBuilder.GetObject<TView>(AbKeyCollection.Ui, assetName, GetLayer(layer));
            // 调用显示函数
            view.Show();
            // 创建数据
            var model = factory?.CreateModel();
            // 创建控制器
            var controller = factory?.CreateController(view, model);
            // 等待控制器初始化
            if (controller == null)
            {
                return null;
            }
            await controller.Init();
            // 初始化面板信息
            var newInfo = new PanelInfo<TView, TModel, TController>(view, model, controller);
            // 存储面板信息
            _panels.Push(newInfo);
            return controller;
#endif
        }
        
        public async void DestroyView(string abName, IuiController controller)
        {
            try
            {
                for (var i = _panels.Count - 1; i >= 0; i--)
                {
                    var uiController = _panels[i].UiController;
                    if (uiController != controller)
                    {
                        continue;
                    }
                
                    // 调用控制器的销毁
                    await uiController.Destroy();
                    ServiceLocator.Get<IPrefabLoader>().CollectAsset(_panels[i].UiView.ViewObj);
                    // 释放该UI的资源
                    ServiceLocator.Get<IPrefabLoader>().RealseAsset(abName, _panels[i].UiView.ViewObj.name);
                    // 从缓存中移除
                    _panels.RemoveAt(i);
                }
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(UIManager)}.{nameof(GetController)}：{e.Message}，{e.StackTrace}");
            }
        }
        
        public async Task SetViewActive(IuiController controller, bool isActive)
        {
            if (_panels.ConvertAll(info => info.UiController).Contains(controller))
            {
                if (!isActive)
                {
                    await controller.Hide();
                }
                else
                {
                    await controller.Show();
                }
            }
        }

        public TController GetController<TController>() where TController : IuiController
        {
            foreach (var basePanelInfo in _panels)
            {
                if (basePanelInfo.UiController is TController controller)
                {
                    return controller;
                }
            }
            LogManager.LogError($"{nameof(UIManager)}.{nameof(GetController)}：控制器{typeof(TController)}未找到");
            return default;
        }
        
        public void Clear(string abName)
        {
            // 销毁画布和摄像机
            ServiceLocator.Get<IPrefabLoader>().CollectAsset(Canvas.gameObject);
            ServiceLocator.Get<IPrefabLoader>().RealseAsset(abName, Canvas.name);
            Canvas = null;
            
            ServiceLocator.Get<IPrefabLoader>().CollectAsset(UICamera.gameObject);
            ServiceLocator.Get<IPrefabLoader>().RealseAsset(abName, UICamera.name);
            UICamera = null;
            
            // 销毁所有界面
            foreach (var panelInfo in _panels)
            {
                DestroyView(abName, panelInfo.UiController);
            }
            _panels.Clear();
        }
        
        public List<IPanelInfo> AllPanels => _panels;

        public Canvas Canvas { get; private set; }

        public Camera UICamera { get; private set; }
    }
}
