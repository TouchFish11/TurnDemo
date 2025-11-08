using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using XLua;

namespace Framework
{
    /// <summary>
    /// UI管理器
    /// </summary>
    [LuaCallCSharp]
    public class UIManager : SingletonBase<UIManager>
    {
        //存储所有面板的字典
        private readonly Dictionary<string, BasePanelInfo> _panelDic = new Dictionary<string, BasePanelInfo>();
        //画布
        private Canvas _canvas;
        //ui摄像机
        private Camera _uiCamera;
        //上层
        private Transform _topLayer;
        //中层
        private Transform _midLayer;
        //底层
        private Transform _botLayer;
        //系统层
        private Transform _systemLayer;

        private UIManager()
        {
#if EDITOR_TEST_AB || !UNITY_EDITOR
            //异步加载Canvas
            AssetBundleLoadManager.Instance.LoadAssetAsync<GameObject>(E_AssetBundleType.UI, "Canvas", (canvasObj) =>
            {
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

                //异步加载UICamera
                AssetBundleLoadManager.Instance.LoadAssetAsync<GameObject>(E_AssetBundleType.Camera, "UICamera", (uiCameraObj) =>
                {
                    //实例化摄像机对象
                    GameObject uiCameraInstance = GameObject.Instantiate(uiCameraObj);
                    //记录UI摄像机
                    _uiCamera = uiCameraInstance.GetComponent<Camera>();
                    //过场景不移除
                    GameObject.DontDestroyOnLoad(uiCameraInstance);
                    //设置UI摄像机
                    _canvas.worldCamera = _uiCamera;
                });
            });
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
        /// 异步加载面板
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        /// <param name="layer">显示层级</param>
        /// <param name="callBack">加载结束回调</param>
        public void ShowPanelAsync<T>(E_UILayer layer, UnityAction<T> callBack) where T : BasePanel
        {
#if EDITOR_TEST_AB || !UNITY_EDITOR
            //自定义存储名称
            string panelName = typeof(T).Name;
            PanelInfo<T> info;
            if (_panelDic.ContainsKey(panelName))
            {
                info = _panelDic[panelName] as PanelInfo<T>;
                //正在异步加载
                if (info.Panel == null)
                    info.CallBack += callBack;
                //资源加载结束，直接使用
                else
                    callBack?.Invoke(info.Panel);
                return;
            }

            //初始化面板信息
            info = new PanelInfo<T>(callBack);
            //存储面板信息
            _panelDic.Add(panelName, info);
            //异步加载面板
            AssetBundleLoadManager.Instance.LoadAssetAsync<GameObject>(E_AssetBundleType.UI, panelName, (panelObj) =>
            {
                if (info.IsDestroy)
                {
                    //从字典中移除
                    _panelDic.Remove(panelName);
                    return;
                }
                //实例化面板对象、设置面板父对象
                GameObject panelInstanceObj = GameObject.Instantiate(panelObj, GetLayer(layer), false);
                //获取面板脚本
                T panel = panelInstanceObj.GetComponent<T>();
                //调用显示函数
                panel.Show();
                //存储面板
                info.Panel = panel;
                //执行存储的回调函数
                info.Invoke(panel);
            });
#else
            //自定义存储名称
            string panelName = typeof(T).Name;
            PanelInfo<T> info = null;
            if (_panelDic.ContainsKey(panelName))
            {
                info = _panelDic[panelName] as PanelInfo<T>;
                callBack?.Invoke(info.Panel);
                return;
            }
            //加载资源
            GameObject panelObj = EditorResMgr.Instance.LoadEditorAsset<GameObject>(panelName);
            //实例化面板对象、设置面板父对象
            GameObject panelInstanceObj = GameObject.Instantiate(panelObj, GetLayer(layer), false);
            //获取面板脚本
            T panel = panelInstanceObj.GetComponent<T>();
            //调用显示函数
            panel.Show();
            info = new PanelInfo<T>(callBack);
            //存储面板
            info.Panel = panel;
            //存储面板信息
            _panelDic.Add(panelName, info);
            //执行存储的回调函数
            info.Invoke(panel);
#endif
        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        public void HidePanel<T>() where T : BasePanel
        {
#if EDITOR_TEST_AB || !UNITY_EDITOR
            //自定义存储名称
            string panelName = typeof(T).Name;
            if (_panelDic.ContainsKey(panelName))
            {
                PanelInfo<T> info = _panelDic[panelName] as PanelInfo<T>;
                //正在异步加载
                if (info.Panel == null)
                    //改变标识
                    info.IsDestroy = true;
                else
                {
                    //调用面板隐藏
                    info.Panel.Hide();
                    //销毁预设体
                    GameObject.Destroy(info.Panel.gameObject);
                    //从字典中移除
                    _panelDic.Remove(panelName);
                }
            }
#else
            //自定义存储名称
            string panelName = typeof(T).Name;
            if (_panelDic.ContainsKey(panelName))
            {
                PanelInfo<T> info = _panelDic[panelName] as PanelInfo<T>;
                //调用面板隐藏
                info.Panel.Hide();
                //销毁预设体
                GameObject.Destroy(info.Panel.gameObject);
                //从字典中移除
                _panelDic.Remove(panelName);
            }
#endif
        }

        /// <summary>
        /// 获取面板
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        /// <param name="callBack">回调函数</param>
        public void GetPanel<T>(UnityAction<T> callBack) where T : BasePanel
        {
#if EDITOR_TEST_AB || !UNITY_EDITOR
            //自定义存储名称
            string panelName = typeof(T).Name;
            if (_panelDic.ContainsKey(panelName))
            {
                PanelInfo<T> info = _panelDic[panelName] as PanelInfo<T>;
                if (info.Panel == null)
                    info.CallBack += callBack;
                else
                    callBack?.Invoke(info.Panel);
            }
#else
            //自定义存储名称
            string panelName = typeof(T).Name;
            if (_panelDic.ContainsKey(panelName))
            {
                PanelInfo<T> info = _panelDic[panelName] as PanelInfo<T>;
                callBack?.Invoke(info.Panel);
            }
            else
            {
                callBack?.Invoke(null);
            }
#endif
        }

        /// <summary>
        /// 添加自定义事件监听
        /// </summary>
        /// <param name="control">要监听的控件</param>
        /// <param name="type">事件类型</param>
        /// <param name="listener">监听函数</param>
        public void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> listener)
        {
            if (!control.TryGetComponent<EventTrigger>(out var eventTrigger))
                eventTrigger = control.AddComponent<EventTrigger>();

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
        /// <param name="target">转换目标</param>
        /// <param name="worldPoint">世界点</param>
        /// <param name="offset">UI坐标偏移</param>
        /// <param name="localPoint">本地UI点</param>
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
                LogMgr.Log("转换失败");
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
