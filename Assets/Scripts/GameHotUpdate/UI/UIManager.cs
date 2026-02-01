using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Config;
using Core.Log;
using Core.Service;
using Core.Singleton;
using Core.UI;
using Core.UI.MVC;
using Game.Objects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameHotUpdate.UI
{
    /// <summary>
    /// UI管理器
    /// </summary>
    public class UIManager : SingletonBase<UIManager>, IUIManager
    {
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

        private UIManager()
        {

        }

        /// <summary>
        /// 异步初始化UI管理器
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task InitUIManagerAsync()
        {
#if EDITOR_TEST_AB || !UNITY_EDITOR
            // 加载画布资源
            var canvasObj = await AssetBundleManager.Instance.LoadAssetAsync<GameObject>(EAssetBundleType.UI, ResKeyCollection.Canvas);
            // 实例化画布对象
            var canvasInstance = Object.Instantiate(canvasObj);
            // 记录画布对象
            Canvas = canvasInstance.GetComponent<Canvas>();
            // 过场景不移除
            Object.DontDestroyOnLoad(canvasInstance);

            // 获取对应层级对象位置
            _topLayer = Canvas.transform.Find("Top");
            _midLayer = Canvas.transform.Find("Mid");
            _botLayer = Canvas.transform.Find("Bot");   
            _systemLayer = Canvas.transform.Find("System");

            // 加载UI摄像机资源
            var uiCameraObj = await AssetBundleManager.Instance.LoadAssetAsync<GameObject>(EAssetBundleType.Camera, ResKeyCollection.UICamera);
            // 实例化摄像机对象
            var uiCameraInstance = Object.Instantiate(uiCameraObj);
            // 记录UI摄像机
            UICamera = uiCameraInstance.GetComponent<Camera>();
            // 过场景不移除
            Object.DontDestroyOnLoad(uiCameraInstance);
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

        /// <summary>
        /// 获取指定层级对象
        /// </summary>
        /// <param name="layer">层级对象枚举</param>
        /// <returns>层级对象位置</returns>
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

        /// <summary>
        /// 异步显示界面
        /// 可创建同一类型多实例
        /// </summary>
        /// <typeparam name="TView">热更类型</typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TController"></typeparam>
        /// <param name="layer"></param>
        /// <param name="panelName"></param>
        /// <returns></returns>
        public async Task<TController> CreateViewAsync<TView, TModel, TController>(E_UILayer layer, string panelName)
            where TView : BaseUIBehaviour, IuiView where TModel : IuiModel, new() where TController : class, IuiController, new()
        {
#if EDITOR_TEST_AB || !UNITY_EDITOR
            // 获取面板
            var view = await ServiceLocator.Get<IObjectBuilder>().GetHotfixUIObject<TView>(EAssetBundleType.UI, panelName, GetLayer(layer));
            view.Show();
            var model = new TModel();
            var controller = new TController();
            // 初始化控制器
            await controller.Init(view, model);
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
            var view = await ObjectBuilder.GetObject<TView>(EAssetBundleType.UI, assetName, GetLayer(layer));
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

        /// <summary>
        /// 销毁界面
        /// 指定实例销毁
        /// </summary>
        public void DestroyView(IuiController controller)
        {
            for (var i = _panels.Count - 1; i >= 0; i--)
            {
                if (_panels[i].UiController != controller)
                {
                    continue;
                }
                
                // 调用面板隐藏
                _panels[i].UiView.Hide();
                // 调用控制器的销毁
                _panels[i].UiController.Destroy();
                // 销毁预设体
                Object.Destroy(_panels[i].UiView.ViewObj);
                // 从缓存中移除
                _panels.RemoveAt(i);
            }
        }

        /// <summary>
        /// 设置界面活动状态
        /// 只能设置第一个查找到的实例，多实例无法准确获取
        /// </summary>
        /// <typeparam name="TController"></typeparam>
        /// <param name="isActive"></param>
        public void SetViewActive<TController>(bool isActive) where TController : IuiController
        {
            foreach (var panelInfo in _panels)
            {
                if (panelInfo.UiController is not TController)
                {
                    continue;
                }
                
                var view = panelInfo.UiView;
                if (!isActive)
                {
                    view.Hide();
                    view.ViewObj.SetActive(false);
                }
                else
                {
                    view.ViewObj.SetActive(true);
                    view.Show();
                }
                
                return;
            }
        }

        /// <summary>
        /// 获取界面控制器
        /// 只能获取第一个查找到的实例，多实例无法准确获取
        /// </summary>
        /// <typeparam name="TController">接口类型</typeparam>
        /// <returns></returns>
        public TController GetController<TController>() where TController : IuiController
        {
            foreach (var basePanelInfo in _panels)
            {
                if (basePanelInfo.UiController is TController controller)
                {
                    return controller;
                }
            }
            LogManager.LogError($"控制器未找到“{typeof(TController)}");
            return default;
        }

        /// <summary>
        /// UI画布
        /// </summary>
        public Canvas Canvas { get; private set; }

        /// <summary>
        /// UI摄像机
        /// </summary>
        public Camera UICamera { get; private set; }
    }
}
