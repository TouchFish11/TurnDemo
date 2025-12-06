using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背景界面控制器工厂
/// </summary>
public class BackControllerFactory : UIControllerFactory<BackView, BackModel, BackController>
{
    public override BackModel CreateModel()
    {
        return new BackModel();
    }

    public override BackController CreateController(BackView view, BackModel model)
    {
        return new BackController(view, model);
    }
}

/// <summary>
/// 背景界面控制器
/// </summary>
public class BackController : UIController<BackView, BackModel>
{
    public BackController(BackView view, BackModel model) : base(view, model)
    {

    }

    protected override void OnInit()
    {
        throw new System.NotImplementedException();
    }
}
