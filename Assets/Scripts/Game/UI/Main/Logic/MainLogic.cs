using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主界面逻辑类
/// </summary>
public abstract class MainLogic
{
    protected MainController mainController;
    protected MainModel mainModel;
    protected MainView mainView;

    public MainLogic(MainController mainController, MainModel mainModel, MainView mainView)
    {
        this.mainController = mainController;
        this.mainModel = mainModel;
        this.mainView = mainView;
    }
}
