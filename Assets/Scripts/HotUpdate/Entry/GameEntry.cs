using System;
using Core.AssetBundles.Management;
using Core.HotUpdate;
using Core.Input.ActionAsset;
using Core.Log;
using Core.Scene;
using Core.Service;
using Core.UI;
using HotUpdate.Common;
using HotUpdate.Entry.Update.UI;

namespace HotUpdate.Entry
{
    public class GameEntry
    {        
        public static async void Run()
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
                    LogManager.Log($"初始化AB包管理器，加载本地AB包资源成功");
            
                    // 重新初始化UI管理器
                    await ServiceLocator.Get<IUIManager>().InitUIManagerAsync(
                        AbKeyCollection.Default, ResKeyCollection.Canvas, ResKeyCollection.UICamera);
                    // 初始化场景
                    await ServiceLocator.Get<ISceneManager>().InitAsync(AbKeyCollection.Scene);
                    // 初始化输入系统
                    await ServiceLocator.Get<IInputSystem>().InitInputsystemAsync(AbKeyCollection.Gameconfig);
                    // 加载热更程序集，应该更新后再加载程序集
                    await ServiceLocator.Get<IHotUpdateManager>().LoadAssemblysAsync(AbKeyCollection.Hotupdate);
                    LogManager.Log($"加载热更程序集成功");

                    var assembly = ServiceLocator.Get<IHotUpdateManager>().GetAssembly("");
                    Type type = assembly.GetType("");
                    var methodInfo = type.GetMethod("Init");
                    methodInfo?.Invoke(null, null);
                };
            
                // 检查更新
                controller.CheckUpdate();
            
                {
                    // 
                    // ...
                    // 更新后重启
                    // // 获取当前程序的路径和参数
                    // var exePath = Process.GetCurrentProcess().MainModule?.FileName;
                    // var arguments = Environment.CommandLine.Replace($"\"{exePath}\"", "").Trim();
                    // // 启动新进程
                    // Process.Start(new ProcessStartInfo(exePath, arguments));
                    // // 关闭当前进程
                    // Application.Quit();
                }
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(GameEntry)}.{nameof(Run)}：{e.Message}，{e.StackTrace}");
            }
        }

    }
}
