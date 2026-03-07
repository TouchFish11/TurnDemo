using System;
using Core.Log;
using Core.Reflection;
using Core.Scene;
using Core.Service;
using Core.UI;
using HotUpdate.Common;
using HotUpdate.Core.Main;
using HotUpdate.Core.Manager;
using HotUpdate.Main.UI;
using UnityEngine.SceneManagement;

namespace HotUpdate.Main
{
    /// <summary>
    /// 游戏入口代理
    /// </summary>
    public class MainProxy
    {
        /// <summary>
        /// 游戏启动入口方法
        /// </summary>
        private static async void Init()
        {
            try
            {
                // 注册游戏业务层管理器到服务容器
                ServiceLocator.Register<IGameManager>(GameManager.Instance);
                // 初始化热更工厂
                ServiceLocator.Get<IFactoryManager>().InitHotFactorys();
                // 切换场景
                await ServiceLocator.Get<ISceneManager>().LoadSceneAsync(ResKeyCollection.MainScene, LoadSceneMode.Single, null);
                // 初始化游戏数据、服务
                await ServiceLocator.Get<IGameManager>().Init();
                // 初始化场景
                await SceneGenerator.InitMainScene();
                // 创建玩家对象（参数为玩家配置ID，对应玩家基础配置表）
                await ServiceLocator.Get<IPlayerManager>().CreatePlayer(1001);
                // 初始化主界面
                await ServiceLocator.Get<IUIManager>().CreateViewAsync<MainView, MainModel, MainController>(AbKeyCollection.Ui, E_UILayer.Mid, ResKeyCollection.MainView);
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(Main)}.{nameof(Init)}: {e.Message}，{e.StackTrace}");
            }
        }
    }
}
