using System;
using System.Reflection;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.HotUpdate;
using Core.Input.ActionAsset;
using Core.Reflection;
using Core.Scene;
using Core.Serialize.Json;
using Core.UI;
using Core.Utility;
using HotUpdate.Update.Update.UI;
using UnityEditor;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Update
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
                // 初始化游戏设置
                await InitSettings();
                // 初始化UI管理器，创建画布和UI相机
                await DIContainer.GetInstance<IUIManager>().InitUIManagerAsync("");
                // 显示开始界面
                var controller = await DIContainer.GetInstance<IUIManager>().CreateViewAsync<BeginView, BeginController>("", E_UILayer.Mid);
                // 进入游戏
                controller.OnClickEnterGame += EnterGame;
                // 检查更新
                controller.CheckUpdate();
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(HotUpdateEntry)}.{nameof(Run)}:热更新入口运行错误,{e.Message}");
            }
        }

        /// <summary>
        /// 初始化设置
        /// </summary>
        private static Task InitSettings()
        {
            return Task.CompletedTask;
            // var textAsset = await DIContainer.GetInstance<ITextLoader>().LoadAssetAsync(AbKeyCollection.Gameconfig, ResKeyCollection.GameSettingsConfig);
            // var settingsConfig = DIContainer.GetInstance<IJsonManager>().FromJson<GameSettingsConfig>(textAsset.text);
            // var settings = await DIContainer.GetInstance<IJsonManager>()
            //     .FromJsonAsync<GameSettings>(PathUtility.GetUserDataLocalSavePath(FileUtility.GameSettingFileName), settings:NewtonsoftJsonUtility.SerializerSettings);
            //
            // SettingsService.SetFrameRate(settingsConfig.framerates[(int)settings[ESettingType.TargetFrameRateIndex]]);
            // Application.runInBackground = true;
            // ...
        }
        
        /// <summary>
        /// 进入游戏
        /// </summary>
        private Task EnterGame()
        {
            try
            {
                // // 初始化AB包管理器，加载本地AB包资源
                // await DIContainer.GetInstance<IAssetBundleManager>().Init();
                // Logger.Log($"{nameof(HotUpdateEntry)}.{nameof(EnterGame)}:Initialization of the AssetBundleManager is complete");
                // // 重新初始化UI管理器
                // await DIContainer.GetInstance<IUIManager>().InitUIManagerAsync(AbKeyCollection.Default, ResKeyCollection.Canvas, ResKeyCollection.UICamera);
                // LogManager.Log($"{nameof(HotUpdateEntry)}.{nameof(EnterGame)}:Initialization of the UIManager is complete");
                // // 初始化场景管理器
                // await DIContainer.GetInstance<ISceneManager>().InitAsync(AbKeyCollection.Scene);
                // LogManager.Log($"{nameof(HotUpdateEntry)}.{nameof(EnterGame)}:Initialization of the SceneManager is complete");
                // // 初始化输入系统
                // await DIContainer.GetInstance<IInputSystem>().InitInputsystemAsync(AbKeyCollection.Gameconfig);
                // LogManager.Log($"{nameof(HotUpdateEntry)}.{nameof(EnterGame)}:Initialization of the InputSystem is complete");
                // // 加载热更程序集
                // await DIContainer.GetInstance<IHotUpdateManager>().LoadAssembliesAsync(AbKeyCollection.Hotupdate);
                // LogManager.Log($"{nameof(HotUpdateEntry)}.{nameof(EnterGame)}:Load the hotfix assemblies complete");
                // // 初始化热更模块管理器
                // var moduleManager = new ModuleManager(DIContainer.GetInstance<IHotUpdateManager>());
                // DIContainer.GetInstance.Register<IModuleManager>(moduleManager);
                // await moduleManager.InitModules();
                // LogManager.Log($"{nameof(HotUpdateEntry)}.{nameof(EnterGame)}:Initialization of the ModuleManager is complete");
                // // 初始化热更工厂
                // DIContainer.GetInstance<IFactoryManager>().InitHotFactorys();
                // LogManager.Log($"{nameof(HotUpdateEntry)}.{nameof(EnterGame)}:Initialization of the FactoryManager_HotFactorys is complete");
                // // 初始化游戏数据
                // await DIContainer.GetInstance<IGameManager>().InitDataAsync();
                // LogManager.Log($"{nameof(HotUpdateEntry)}.{nameof(EnterGame)}:Initialization of the GameData is complete");
                // 加载代理
                LoadMainProxy();
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(HotUpdateEntry)}.{nameof(EnterGame)}:进入游戏错误，{e.Message}");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 加载游戏主入口代理
        /// </summary>
        private static void LoadMainProxy()
        {
            // 获取游戏主入口代理，初始化主场景
            var assembly = DIContainer.GetInstance<IHotUpdateManager>().GetAssembly("HotUpdate.Main");
            var type = assembly.GetType("HotUpdate.Main.MainProxy");
            var methodInfo = type.GetMethod("Init", BindingFlags.Static | BindingFlags.NonPublic);
            methodInfo?.Invoke(null, null);
        }
    }
}
