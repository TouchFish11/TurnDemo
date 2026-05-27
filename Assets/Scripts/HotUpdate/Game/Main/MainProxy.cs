using System;
using System.Threading.Tasks;
using Core.DI;
using Core.Scene;
using HotUpdate.Base.Scene;

using UnityEngine.SceneManagement;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Main
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
                await CreatePlayerAsync();
                await CreateInitPanelAsync();
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(MainProxy)}.{nameof(Init)}：初始化错误，{e.Message}");
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
                await DIContainer.GetInstance<IPlayerManager>().CreatePlayer(1001);
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(MainProxy)}.{nameof(CreatePlayerAsync)}: 异步创建玩家错误，{e.Message}");
            }
        }

        /// <summary>
        /// 异步创建初始界面
        /// </summary>
        private static Task CreateInitPanelAsync()
        {
            return Task.CompletedTask;
            // // 初始化全局消息界面
            // await DIContainer.GetInstance<IUIManager>()
            //     .CreateViewAsync<GlobalMessageView, GlobalMessageModel, GlobalMessageController>(AbKeyCollection.Ui,
            //         E_UILayer.Bot, AssetKeys.GlobalMessageView, new Vector2(0, 299));
            // // 初始化主界面
            // await DIContainer.GetInstance<IUIManager>()
            //     .CreateViewAsync<MainView, MainModel, MainController>(AbKeyCollection.Ui, 
            //         E_UILayer.Mid, AssetKeys.MainView);
        }
    }
}
