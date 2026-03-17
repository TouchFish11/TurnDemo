using System;
using System.Threading.Tasks;
using Core.Log;
using Core.Scene;
using Core.Service;
using Core.UI;
using HotUpdate.Common;
using HotUpdate.Core.Main;
using HotUpdate.Core.Scene;
using HotUpdate.Main.Global.UI;
using HotUpdate.Main.UI;
using UnityEngine;
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
                await LoadSceneAsync();
                await CreatePlayerAsync();
                await CreateInitPanelAsync();
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(MainProxy)}.{nameof(Init)}：初始化错误，{e.Message}");
            }
        }

        /// <summary>
        /// 异步加载主场景
        /// </summary>
        private static async Task LoadSceneAsync()
        {
            try
            {
                // 切换到主场景
                await ServiceLocator.Get<ISceneManager>().LoadSceneAsync(ResKeyCollection.MainScene, LoadSceneMode.Single, null);
                // 初始化场景
                await SceneGeneratorHelper.GetSceneGenerator().InitMainScene();
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(MainProxy)}.{nameof(LoadSceneAsync)}: 加载主场景错误，{e.Message}");
            }
        }

        /// <summary>
        /// 异步创建玩家
        /// </summary>
        private static async Task CreatePlayerAsync()
        {
            try
            {
                // 创建玩家对象（参数为玩家配置ID，对应玩家基础配置表）
                await ServiceLocator.Get<IPlayerManager>().CreatePlayer(1001);
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(MainProxy)}.{nameof(CreatePlayerAsync)}: 异步创建玩家错误，{e.Message}");
            }
        }

        /// <summary>
        /// 异步创建初始界面
        /// </summary>
        private static async Task CreateInitPanelAsync()
        {
            try
            {
                // 初始化全局消息界面
                await ServiceLocator.Get<IUIManager>()
                    .CreateViewAsync<GlobalMessageView, GlobalMessageModel, GlobalMessageController>(AbKeyCollection.Ui,
                        E_UILayer.Bot, ResKeyCollection.GlobalMessageView, new Vector2(0, 299));
                // 初始化主界面
                await ServiceLocator.Get<IUIManager>()
                    .CreateViewAsync<MainView, MainModel, MainController>(AbKeyCollection.Ui, 
                        E_UILayer.Mid, ResKeyCollection.MainView);
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(MainProxy)}.{nameof(CreateInitPanelAsync)}: 创建初始界面错误，{e.Message}");
            }
        }
    }
}
