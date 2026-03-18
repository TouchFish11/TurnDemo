using System;
using System.Reflection;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.HotUpdate;
using Core.Input.ActionAsset;
using Core.Log;
using Core.Reflection;
using Core.Scene;
using Core.Service;
using Core.UI;
using HotUpdate.Common;
using HotUpdate.Core.Manager;
using HotUpdate.Core.Module;
using HotUpdate.Entry.Update.UI;
using UnityEngine;

namespace HotUpdate.Entry
{
    /// <summary>
    /// 热更新入口
    /// </summary>
    public class HotUpdateEntry : MonoBehaviour
    {
        private void Start()
        {
            Run();
        }

        /// <summary>
        /// 运行
        /// </summary>
        private async void Run()
        {
            try
            {
                // 初始化UI管理器，创建画布和UI相机
                await ServiceLocator.Get<IUIManager>().InitUIManagerAsync(AbKeyCollection.Default, ResKeyCollection.Canvas, ResKeyCollection.UICamera);
                // 显示开始界面
                var controller = await ServiceLocator.Get<IUIManager>().CreateViewAsync<BeginView, BeginModel, BeginController>(AbKeyCollection.Default, E_UILayer.Mid, ResKeyCollection.BeginView);
                // 进入游戏
                controller.OnClickEnterGame += EnterGame;
                // 检查更新
                controller.CheckUpdate();
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(HotUpdateEntry)}.{nameof(Run)}:热更新入口运行错误,{e.Message}");
            }
        }

        /// <summary>
        /// 进入游戏
        /// </summary>
        private async Task EnterGame()
        {
            try
            {
                // 初始化AB包管理器，加载本地AB包资源
                await ServiceLocator.Get<IAssetBundleManager>().Init();
                LogManager.Log($"{nameof(HotUpdateEntry)}.{nameof(EnterGame)}:Initialization of the AssetBundleManager is complete");
                // 重新初始化UI管理器
                await ServiceLocator.Get<IUIManager>().InitUIManagerAsync(AbKeyCollection.Default, ResKeyCollection.Canvas, ResKeyCollection.UICamera);
                LogManager.Log($"{nameof(HotUpdateEntry)}.{nameof(EnterGame)}:Initialization of the UIManager is complete");
                // 初始化场景管理器
                await ServiceLocator.Get<ISceneManager>().InitAsync(AbKeyCollection.Scene);
                LogManager.Log($"{nameof(HotUpdateEntry)}.{nameof(EnterGame)}:Initialization of the SceneManager is complete");
                // 初始化输入系统
                await ServiceLocator.Get<IInputSystem>().InitInputsystemAsync(AbKeyCollection.Gameconfig);
                LogManager.Log($"{nameof(HotUpdateEntry)}.{nameof(EnterGame)}:Initialization of the InputSystem is complete");
                // 加载热更程序集
                await ServiceLocator.Get<IHotUpdateManager>().LoadAssembliesAsync(AbKeyCollection.Hotupdate);
                LogManager.Log($"{nameof(HotUpdateEntry)}.{nameof(EnterGame)}:Load the hotfix assemblies complete");
                // 初始化热更模块管理器
                var moduleManager = new ModuleManager(ServiceLocator.Get<IHotUpdateManager>());
                ServiceLocator.Register<IModuleManager>(moduleManager);
                await moduleManager.InitModules();
                LogManager.Log($"{nameof(HotUpdateEntry)}.{nameof(EnterGame)}:Initialization of the ModuleManager is complete");
                // 初始化热更工厂
                ServiceLocator.Get<IFactoryManager>().InitHotFactorys();
                LogManager.Log($"{nameof(HotUpdateEntry)}.{nameof(EnterGame)}:Initialization of the FactoryManager_HotFactorys is complete");
                // 初始化游戏数据
                await ServiceLocator.Get<IGameManager>().InitDataAsync();
                LogManager.Log($"{nameof(HotUpdateEntry)}.{nameof(EnterGame)}:Initialization of the GameData is complete");
                // 加载代理
                LoadMainProxy();
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(HotUpdateEntry)}.{nameof(EnterGame)}:进入游戏错误，{e.Message}");
            }
        }

        /// <summary>
        /// 加载游戏主入口代理
        /// </summary>
        private static void LoadMainProxy()
        {
            // 获取游戏主入口代理，初始化主场景
            var assembly = ServiceLocator.Get<IHotUpdateManager>().GetAssembly("HotUpdate.Main");
            var type = assembly.GetType("HotUpdate.Main.MainProxy");
            var methodInfo = type.GetMethod("Init", BindingFlags.Static | BindingFlags.NonPublic);
            methodInfo?.Invoke(null, null);
        }
    }
}
