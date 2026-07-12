using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.HotUpdate;
using Core.Log;
using Core.Registration;
using Core.Serialize.Json;
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
                // 注册框架
                await RegisterCore.InitCore();
                // 加载启动配置
                LoadLaunchConfig();
                // 加载热更程序集
                await LoadHotfixDll();
                // 创建热更入口
                using var spawner = DIContainer.Create<ObjectSpawner>();
                await spawner.SpawnAsync<GameObject>(bootConfig.hotfixObjKey);
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.GameLauncher, $"Game startup failed, {e}");
            }
        }
        
        /// <summary>
        /// 加载热更程序集
        /// </summary>
        private async Task LoadHotfixDll()
        {
            if (bootConfig == null)
            {
                Logger.LogError(ELogTags.GameLauncher, $"无法加载启动配置，使用默认硬编码包名");
                bootConfig = new BootConfig
                {
                    aotDllBundleName = "hotupdateaot.assetbundle",
                    hotfixDllBundleName = "hotupdate.assetbundle"
                };
            }

            IHotUpdateManager hotUpdateManager;
#if UNITY_EDITOR
            hotUpdateManager = DIContainer.Resolve<HotUpdateMockManager>();
#else
            hotUpdateManager = DIContainer.Resolve<HotUpdateManager>();
            // 补充元数据
            using var aotDllHandle = await GameAsset.LoadAllAssetAsync<TextAsset>(bootConfig.aotDllBundleName);
            var dllDic = new Dictionary<string, byte[]>();
            foreach (var asset in aotDllHandle.Assets)
            {
                dllDic.Add(asset.name, asset.bytes);
            }
            hotUpdateManager.LoadMetadataForAOTAssemblies(dllDic); 
#endif
            
            // 加载所有热更dll资源
            using var handle = await GameAsset.LoadAllAssetAsync<TextAsset>(bootConfig.hotfixDllBundleName);
            var list = new List<TextAsset>(handle.Assets);
            // 获取热更程序集依赖设置
            var settingsTextAsset = list.Find(text => text.name.Contains(nameof(HotUpdateAssemblySettings)));
            list.Remove(settingsTextAsset);
            var settings = DIContainer.Resolve<JsonManager>().FromJson<HotUpdateAssemblySettings>(settingsTextAsset.text);
            // 加载所有热更程序集
            await hotUpdateManager.LoadAssembliesAsync(settings, list);
            Logger.LogDebug(ELogTags.GameLauncher, $"Load the hotfix assemblies complete");
        }
        
        /// <summary>
        /// 加载启动配置
        /// </summary>
        private void LoadLaunchConfig()
        {
            var jsonManager = DIContainer.Resolve<IJsonManager>();
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
