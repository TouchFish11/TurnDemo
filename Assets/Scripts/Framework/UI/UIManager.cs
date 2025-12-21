using Game.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using XLua;

namespace Framework
{
    /// <summary>
    /// UI管理器
    /// </summary>
    [LuaCallCSharp]
    public class UIManager : SingletonBase<UIManager>
    {
        // 存储打开的界面
        private readonly Stack<BasePanelInfo> _panels = new Stack<BasePanelInfo>();

        // UI控制器类型到UI控制器工程接口映射 
        private readonly Dictionary<Type, IUIControllerFactory> _typeToCtrlFactoryMap = new Dictionary<Type, IUIControllerFactory>();
        // 画布
        private Canvas _canvas;
        // ui摄像机
        private Camera _uiCamera;
        // 上层
        private Transform _topLayer;
        // 中层
        private Transform _midLayer;
        // 底层
        private Transform _botLayer;
        // 系统层
        private Transform _systemLayer;

        private UIManager() { }

        /// <summary>
        /// 异步初始化UI管理器
        /// </summary>
        /// <returns></returns>
        public async Task InitUIManagerAsync()
        {
            RegisterControllerFactory();

#if EDITOR_TEST_AB || !UNITY_EDITOR
            // 加载画布资源
            GameObject canvasObj = await AssetBundleManager.Instance.LoadAssetAsync<GameObject>(E_AssetBundleType.UI, "Canvas");
            // 实例化画布对象
            GameObject canvasInstance = GameObject.Instantiate(canvasObj);
            // 记录画布对象
            _canvas = canvasInstance.GetComponent<Canvas>();
            // 过场景不移除
            GameObject.DontDestroyOnLoad(canvasInstance);

            // 获取对应层级对象位置
            _topLayer = _canvas.transform.Find("Top");
            _midLayer = _canvas.transform.Find("Mid");
            _botLayer = _canvas.transform.Find("Bot");
            _systemLayer = _canvas.transform.Find("System");

            // 加载UI摄像机资源
            GameObject uiCameraObj = await AssetBundleManager.Instance.LoadAssetAsync<GameObject>(E_AssetBundleType.Camera, "UICamera");
            // 实例化摄像机对象
            GameObject uiCameraInstance = GameObject.Instantiate(uiCameraObj);
            // 记录UI摄像机
            _uiCamera = uiCameraInstance.GetComponent<Camera>();
            // 过场景不移除
            GameObject.DontDestroyOnLoad(uiCameraInstance);
            // 设置UI摄像机
            _canvas.worldCamera = _uiCamera;
#else
            //加载画布资源
            GameObject canvasObj = EditorResMgr.Instance.LoadEditorAsset<GameObject>("Canvas");
            //实例化画布对象
            GameObject canvasInstance = GameObject.Instantiate(canvasObj);
            //记录画布对象
            _canvas = canvasInstance.GetComponent<Canvas>();
            //过场景不移除
            GameObject.DontDestroyOnLoad(canvasInstance);
            //获取对应层级对象位置
            _topLayer = _canvas.transform.Find("Top");
            _midLayer = _canvas.transform.Find("Mid");
            _botLayer = _canvas.transform.Find("Bot");
            _systemLayer = _canvas.transform.Find("System");
            //加载UI摄像机资源
            GameObject uiCameraObj = EditorResMgr.Instance.LoadEditorAsset<GameObject>("UICamera");
            //实例化摄像机对象
            GameObject uiCameraInstance = GameObject.Instantiate(uiCameraObj);
            //记录UI摄像机
            _uiCamera = uiCameraInstance.GetComponent<Camera>();
            //过场景不移除
            GameObject.DontDestroyOnLoad(uiCameraInstance);
            //设置UI摄像机
            _canvas.worldCamera = _uiCamera;

            await Task.CompletedTask;
#endif
        }

        /// <summary>
        /// 注册UI控制器工厂
        /// </summary>
        private void RegisterControllerFactory()
        {
            _typeToCtrlFactoryMap.Add(typeof(LoginController), new LoginControllerFactory());
            _typeToCtrlFactoryMap.Add(typeof(BackController), new BackControllerFactory());
            _typeToCtrlFactoryMap.Add(typeof(BeginController), new BeginControllerFactory());
            _typeToCtrlFactoryMap.Add(typeof(VideoController), new VideoControllerFactory());
            _typeToCtrlFactoryMap.Add(typeof(MainController), new MainControllerFactory());
            _typeToCtrlFactoryMap.Add(typeof(DialogueController), new DialogueControllerFactory());
            _typeToCtrlFactoryMap.Add(typeof(TaskController), new TaskControllerFactory());
            _typeToCtrlFactoryMap.Add(typeof(BattleLoadingController), new BattleLoadingControllerFactory());
            _typeToCtrlFactoryMap.Add(typeof(BattleController), new BattleControllerFactory());
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
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <param name="layer"></param>
        /// <returns></returns>
        public async Task<TController> CreateViewAsync<TView, TModel, TController>(E_UILayer layer)
            where TView : UIView where TModel : UIModel, new() where TController : UIController<TView, TModel>
        {
#if EDITOR_TEST_AB || !UNITY_EDITOR
            // 自定义存储名称
            string cacheName = $"{typeof(TView).Name}";
            // 存在缓存
            if (_panelDic.ContainsKey(cacheName))
            {
                PanelInfo<TView, TModel, TController> cacheInfo = _panelDic[cacheName] as PanelInfo<TView, TModel, TController>;
                // 返回缓存的面板
                return cacheInfo.UIController;
            }

            // 不存在工厂
            if (!_typeToCtrlFactoryMap.TryGetValue(typeof(TController), out var iFactory))
            {
                return null;
            }

            // 存在工厂
            var factory = iFactory as UIControllerFactory<TView, TModel, TController>;
            // 加载面板资源
            GameObject panelObj = await AssetBundleManager.Instance.LoadAssetAsync<GameObject>(E_AssetBundleType.UI, typeof(TView).Name);
            // 实例化面板对象、设置面板父对象
            GameObject panelInstanceObj = GameObject.Instantiate(panelObj, GetLayer(layer), false);
            // 获取面板脚本
            TView view = panelInstanceObj.GetComponent<TView>();
            // 调用显示函数
            view.Show();
            // 创建数据
            TModel model = factory.CreateModel();
            // 创建控制器
            TController controller = factory.CreateController(view, model);
            // 初始化面板信息
            PanelInfo<TView, TModel, TController> newInfo = new PanelInfo<TView, TModel, TController>(view, model, controller);
            // 存储面板信息
            _panelDic.Add(cacheName, newInfo);
            return controller;
#else
            //自定义存储名称
            string assetName = $"{typeof(TView).Name}";
            // 不存在工厂
            if (!_typeToCtrlFactoryMap.TryGetValue(typeof(TController), out var iFactory))
            {
                LogManager.LogWarning($"未初始化{typeof(TController)}控制器工厂");
                return null;
            }

            // 存在工厂
            var factory = iFactory as UIControllerFactory<TView, TModel, TController>;
            // 获取面板
            TView view = await ObjectBuilder.GetOrCreateInstance<TView>(E_AssetBundleType.UI, assetName, GetLayer(layer));
            // 调用显示函数
            view.Show();
            // 创建数据
            TModel model = factory.CreateModel();
            // 创建控制器
            TController controller = factory.CreateController(view, model);
            // 等待控制器初始化
            await controller.Init();
            // 初始化面板信息
            PanelInfo<TView, TModel, TController> newInfo = new PanelInfo<TView, TModel, TController>(view, model, controller);
            // 存储面板信息
            _panels.Push(newInfo);
            return controller;
#endif
        }

        /// <summary>
        /// 销毁界面
        /// </summary>
        public void DestroyView()
        {
            if (_panels.TryPop(out BasePanelInfo basePanelInfo))
            {
                // 调用面板隐藏
                basePanelInfo.View.Hide();
                // 调用控制器的销毁
                basePanelInfo.Controller.Destroy();
                // 销毁预设体
                GameObject.Destroy(basePanelInfo.View.gameObject);
            }
        }

        /// <summary>
        /// 设置界面活动状态
        /// </summary>
        /// <typeparam name="TController"></typeparam>
        /// <param name="isActive"></param>
        public void SetViewActive<TController>(bool isActive) where TController : class, IUIController
        {
            foreach (BasePanelInfo basePanelInfo in _panels)
            {
                if (basePanelInfo.Controller.GetType() == typeof(TController))
                {
                    UIView view = basePanelInfo.View;
                    if (!isActive)
                    {
                        view.Hide();
                        view.gameObject.SetActive(isActive);
                    }
                    else
                    {
                        view.gameObject.SetActive(isActive);
                        view.Show();
                    }
                }
            }
        }

        /// <summary>
        /// 获取界面控制器
        /// </summary>
        /// <typeparam name="TController"></typeparam>
        /// <returns></returns>
        public TController GetView<TController>() where TController : class, IUIController
        {
            foreach (BasePanelInfo basePanelInfo in _panels)
            {
                if (basePanelInfo.Controller.GetType() == typeof(TController))
                {
                    return basePanelInfo.Controller as TController;
                }
            }
            return default;
        }

        /// <summary>
        /// 添加自定义事件监听
        /// </summary>
        /// <param name="control">要监听的控件</param>
        /// <param name="type">事件类型</param>
        /// <param name="listener">监听函数</param>
        public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> listener)
        {
            if (!control.TryGetComponent<EventTrigger>(out var eventTrigger))
            {
                eventTrigger = control.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(listener);
            eventTrigger.triggers.Add(entry);
        }

        /// <summary>
        /// 世界转UI坐标
        /// </summary>
        /// <param name="world">世界摄像机</param>
        /// <param name="ui">UI摄像机</param>
        /// <param name="parent">父对象</param>
        /// <param name="uiObj">世界点</param>
        /// <param name="worldPoint">世界点</param>
        /// <param name="offset">UI坐标偏移</param>
        public bool WorldToLocalPointInRectangle(Camera world, Camera ui, Transform parent, GameObject uiObj, Vector3 worldPoint, Vector2 offset)
        {
            //世界转屏幕
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(world, worldPoint);
            //屏幕转UI
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parent as RectTransform, screenPoint, ui, out Vector2 localPoint))
            {
                //设置父对象
                uiObj.transform.SetParent(parent, false);
                (uiObj.transform as RectTransform).anchoredPosition = localPoint + offset;
                return true;
            }
            else
            {
                LogManager.Log("转换失败");
                return false;
            }
        }

        /// <summary>
        /// UI画布
        /// </summary>
        public Canvas Canvas => this._canvas;

        /// <summary>
        /// UI摄像机
        /// </summary>
        public Camera UICamera => this._uiCamera;
    }
}
