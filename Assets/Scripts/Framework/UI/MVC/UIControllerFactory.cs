using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI控制器工厂
/// </summary>
/// <typeparam name="TView"></typeparam>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TController"></typeparam>
public abstract class UIControllerFactory<TView, TModel, TController> : IUIControllerFactory
    where TView : UIView where TModel : UIModel, new() where TController : UIController<TView, TModel>
{
    /// <summary>
    /// 创建数据模型
    /// </summary>
    /// <returns></returns>
    public abstract TModel CreateModel();

    /// <summary>
    /// 创建控制器
    /// </summary>
    /// <param name="view"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public abstract TController CreateController(TView view, TModel model);

    protected UIControllerFactory()
    {

    }
}