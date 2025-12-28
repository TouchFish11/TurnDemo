using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// UI管理器接口
/// </summary>
public interface IUIManager
{
    Task<TController> CreateViewAsync<TView, TModel, TController>(E_UILayer layer)
        where TView : UIView
        where TModel : UIModel, new()
        where TController : UIController<TView, TModel>;

    void RegisterControllerFactory();
    void DestroyView();
    Transform GetLayer(E_UILayer layer);
    TController GetView<TController>() where TController : class, IUIController;
    Task InitUIManagerAsync();
    void SetViewActive<TController>(bool isActive) where TController : class, IUIController;
}
