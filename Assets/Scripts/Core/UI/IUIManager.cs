using System.Collections.Generic;
using System.Threading.Tasks;
using Core.UI.MVC;
using UnityEngine;

namespace Core.UI
{
    /// <summary>
    /// UI管理器接口
    /// </summary>
    public interface IUIManager
    {
        /// <summary>
        /// UI画布
        /// </summary>
        Canvas Canvas { get; }
        
        /// <summary>
        /// UI摄像机
        /// </summary>
        Camera UICamera { get; }
        
        /// <summary>
        /// 所有存活的界面列表
        /// </summary>
        List<IPanelInfo> AllPanels { get; }

        /// <summary>
        /// 获取指定层级对象
        /// </summary>
        /// <param name="layer">层级对象枚举</param>
        /// <returns>层级对象位置</returns>
        Transform GetLayer(E_UILayer layer);
        
        /// <summary>
        /// 获取界面控制器
        /// 只能获取第一个查找到的实例，同一类型多实例无法准确获取
        /// </summary>
        /// <typeparam name="TController">接口类型</typeparam>
        /// <returns></returns>
        TController GetController<TController>() where TController : IuiController;
        
        /// <summary>
        /// 异步初始化UI管理器
        /// </summary>
        /// <param name="defaultAbName"></param>
        /// <param name="canvasName"></param>
        /// <param name="uiCameraName"></param>
        /// <returns></returns>
        Task InitUIManagerAsync(string defaultAbName, string canvasName, string uiCameraName);
        
        /// <summary>
        /// 销毁界面
        /// </summary>
        void DestroyView(string abName, IuiController controller);

        /// <summary>
        /// 设置界面活动状态
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="isActive"></param>
        Task SetViewActive(IuiController controller, bool isActive);

        /// <summary>
        /// 异步显示界面
        /// 可创建同一类型多实例
        /// </summary>
        /// <typeparam name="TView">热更类型</typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TController"></typeparam>
        /// <param name="abName"></param>
        /// <param name="layer"></param>
        /// <param name="panelName"></param>
        /// <param name="pos"></param>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        Task<TController> CreateViewAsync<TView, TModel, TController>(string abName, E_UILayer layer, string panelName, Vector2 pos = default, Quaternion quaternion = default)
            where TView : UIBehaviourBase, IuiView where TModel : IuiModel, new() where TController : class, IuiController, new();

        /// <summary>
        /// 清理
        /// 销毁所有界面、Canvs、UICamera
        /// </summary>
        /// <param name="abName"></param>
        void Clear(string abName);
    }
}
