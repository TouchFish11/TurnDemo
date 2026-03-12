using System;
using System.Reflection;
using Core.AssetBundles.Management;
using Core.HotUpdate;
using Core.Input.ActionAsset;
using Core.Log;
using Core.Scene;
using Core.Service;
using Core.UI;
using HotUpdate.Common;
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

        public async void Run()
        {
            try
            {
                // 初始化UI管理器，创建画布和UI相机
                await ServiceLocator.Get<IUIManager>().InitUIManagerAsync(AbKeyCollection.Default, ResKeyCollection.Canvas, ResKeyCollection.UICamera);
                // 显示开始界面
                var controller = await ServiceLocator.Get<IUIManager>().CreateViewAsync<BeginView, BeginModel, BeginController>(AbKeyCollection.Default, E_UILayer.Mid, ResKeyCollection.BeginView);
                controller.OnClickEnterGame += async () =>
                {
                    // 更新成功，初始化AB包管理器，加载本地AB包资源
                    await ServiceLocator.Get<IAssetBundleManager>().Init();
                    LogManager.Log($"初始化AB包管理器");
            
                    // 重新初始化UI管理器
                    await ServiceLocator.Get<IUIManager>().InitUIManagerAsync(AbKeyCollection.Default, ResKeyCollection.Canvas, ResKeyCollection.UICamera);
                    // 初始化场景管理器
                    await ServiceLocator.Get<ISceneManager>().InitAsync(AbKeyCollection.Scene);
                    // 初始化输入系统
                    await ServiceLocator.Get<IInputSystem>().InitInputsystemAsync(AbKeyCollection.Gameconfig);
                    // 加载热更程序集
                    await ServiceLocator.Get<IHotUpdateManager>().LoadAssembliesAsync(AbKeyCollection.Hotupdate);
                    LogManager.Log($"Load the hotfix assemblies");

                    var assembly = ServiceLocator.Get<IHotUpdateManager>().GetAssembly("HotUpdate.Main");
                    var type = assembly.GetType("HotUpdate.Main.MainProxy");
                    var methodInfo = type.GetMethod("Init", BindingFlags.Static | BindingFlags.NonPublic);
                    methodInfo?.Invoke(null, null);
                };
            
                // 检查更新
                controller.CheckUpdate();
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(HotUpdateEntry)}.{nameof(Run)}：{e.Message}，{e.StackTrace}");
            }
        }

    }
}
