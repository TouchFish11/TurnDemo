using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.HotUpdate;
using Core.Registration;
using Core.Serialize.Json;
using Game.Module;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace Game
{
    /// <summary>
    /// 游戏启动器
    /// </summary>
    public class GameLauncher : MonoBehaviour
    {
        [SerializeField] private string bootConfigFileName = "BootConfig.json";
        private BootConfig bootConfig;
        
        private async void Start()
        {
            try
            {
                await RegisterGame();
                // 加载启动配置
                LoadLaunchConfig();
                // 加载热更程序集
                await LoadHotfixDll();
                // 创建热更入口
                using var spawner = DIContainer.Create<ObjectSpawner>();
                var entryObj = await spawner.SpawnAsync<GameObject>(bootConfig.hotfixObjKey);
                DIContainer.InjectIntoInstance(entryObj.Obj);
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(GameLauncher)}: Game startup failed, {e.Message}");
            }
        }

        private static async Task RegisterGame()
        {
            // 注册框架
            await RegisterCore.InitCore();
            // 初始化模块服务
            DIContainer.Create<ModuleService>(true);
        }
        
        /// <summary>
        /// 加载热更程序集
        /// </summary>
        private async Task LoadHotfixDll()
        {
            if (bootConfig == null)
            {
                Logger.LogError($"{nameof(GameLauncher)}:无法加载启动配置，使用默认硬编码包名");
                bootConfig = new BootConfig { hotfixDllBundleName = "hotupdate.assetbundle" };
            }
            
            // 加载所有dll资源
            using var handle = await GameAsset.LoadAllAssetAsync<TextAsset>(bootConfig.hotfixDllBundleName);
            var list = new List<TextAsset>(handle.Assets);
            // 获取热更程序集依赖设置
            var settingsTextAsset = list.Find(text => text.name.Contains(nameof(HotUpdateAssemblySettings)));
            list.Remove(settingsTextAsset);
            
            var settings = DIContainer.Create<JsonManager>().FromJson<HotUpdateAssemblySettings>(settingsTextAsset.text);
            var hotUpdateManager = DIContainer.Create<HotUpdateMockManager>();
            // 补充元数据
            hotUpdateManager.LoadMetadataForAOTAssemblies(AOTGenericReferences.PatchedAOTAssemblyList);  
            // 加载所有热更程序集
            await hotUpdateManager.LoadAssembliesAsync(settings, list);
            Logger.Log($"{nameof(GameLauncher)}: Load the hotfix assemblies complete!!!");
        }
        
        /// <summary>
        /// 加载启动配置
        /// </summary>
        private void LoadLaunchConfig()
        {
            var jsonManager = DIContainer.Create<IJsonManager>();
            // 优先从持久化目录读取（热更可能更新配置，但通常不需要）
            var persistentPath = Path.Combine(Application.persistentDataPath, bootConfigFileName);
            if (File.Exists(persistentPath))
            {
                var json = File.ReadAllText(persistentPath);
                bootConfig = jsonManager.FromJson<BootConfig>(json);
            }

            // 其次从 StreamingAssets 读取（首包内置）
            var streamingPath = Path.Combine(Application.streamingAssetsPath, bootConfigFileName);
            if (File.Exists(streamingPath))
            {
                var json = File.ReadAllText(streamingPath);
                bootConfig = jsonManager.FromJson<BootConfig>(json);
            }
        }
    }
}
