using System.Collections.Generic;
using System.Threading.Tasks;
using Core.UI.ViewController;
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
        /// 所有显示的界面
        /// </summary>
        Dictionary<int, IPanelInfo>.ValueCollection Panels { get; }

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
        /// <typeparam name="TController">界面类型</typeparam>
        /// <returns></returns>
        TController GetController<TController>() where TController : IuiController;

        /// <summary>
        /// 异步初始化UI管理器
        /// </summary>
        /// <param name="uiRoot"></param>
        /// <returns></returns>
        Task InitUIManagerAsync(string uiRoot);
        
        /// <summary>
        /// 销毁界面
        /// </summary>
        Task DestroyView(int panelId);

        /// <summary>
        /// 设置界面活动状态
        /// </summary>
        /// <param name="panelId"></param>
        /// <param name="isActive"></param>
        Task SetViewActive(int panelId, bool isActive);
        
        /// <summary>
        /// 清理
        /// 销毁所有界面、Canvas、UICamera
        /// </summary>
        Task Clear();

        /// <summary>
        /// 显示界面，可创建同一类型多实例
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="layer"></param>
        /// <param name="pos"></param>
        /// <param name="quaternion"></param>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TController"></typeparam>
        /// <returns></returns>
        Task<TController> CreateViewAsync<TView, TController>(string panelName, E_UILayer layer, Vector2 pos = default, Quaternion quaternion = default)
            where TView : UIView, IuiView where TController : class, IuiController;
    }
}
