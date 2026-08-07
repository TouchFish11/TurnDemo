using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Global;
using Core.HotUpdate;
using Core.Serialize.Json;
using Editor.AssetBundle.Core;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Editor.AssetBundle
{
    public class ABPackerWindow : EditorWindow
    {
        // --- 核心模块 ---
        private AssetBundleCollector collector;
        private AssetBundleDiffer differ;
        private AssetBundleDependencyResolver dependencyResolver;
        private AssetBundleBuilder builder;
        private AssetBundleUploader uploader;
        private readonly JsonManager jsonManager = DIContainer.Create<JsonManager>();

        // --- GUI 状态 ---
        private Vector2 scrollPos = Vector2.zero;
        private Vector2 leftScrollPos;
        private string buildLog = "";

        // --- 配置参数 ---
        private BuildTarget targetPlatform = BuildTarget.StandaloneWindows64;
        private BuildAssetBundleOptions buildOptions = BuildAssetBundleOptions.ChunkBasedCompression;
        private string outputPath;
        private const string AB_COPY_PATH = "Assets/StreamingAssets/AssetBundles/";
        private string mainBundlePath = "";
        private string serverIP = "http://...";
        private bool uploadUseUser;
        private string userName = "userName";
        private string password = "password";
        private bool showPassWord;
        private bool uploadBytesIsAutoSetting = true;
        private bool uploadBytesIsCustomSetting;
        private uint maxBytesCapacity = 4096;

        private const string hotUpdateAssemblyTargetPath = @"D:\UnityProject\TurnDemo\Assets\Editor\ArtRes\HotUpdate\";
        private const string hybridCLRAssemblySourcesPath = @"D:\UnityProject\TurnDemo\HybridCLRData\HotUpdateDlls\StandaloneWindows64\";
        private const string AssetsInputPath = "Assets/Editor/ArtRes/";
        private readonly string[] filterDirectories = { "Texture" };
        private readonly string[] filterSuffixes = { ".meta" };
        private const string abSettingsSavePath = "Assets/Editor/AssetBundleSettings/";
        private const string assetCollectionName_Temp = "AssetBundlesCollections_Temp.asset";
        private const string assetCollectionName_Release = "AssetBundlesCollections.asset";
        private readonly string serverDataPath = $"{Application.dataPath}/ServerData/";

        // --- 数据容器 ---
        private AssetBundlesCollections assetsCollection_Temp;
        private AssetBundlesCollections assetsCollection_Release;
        private Dictionary<string, List<AssetBundlesCollections.AssetInfo>> abNameToDifferenceInfos = new();
        private List<string> waitRemoveAbNames = new();
        private Dictionary<string, List<AssetBundlesCollections.AssetInfo>> waitRemoveAssetInfos = new();
        private readonly HashSet<string> forceUploadBundles = new();

        // --- 序列化字段 ---
        private SerializedObject serializedObject;
        private SerializedProperty hotUpdateAssembliesProp;
        [SerializeField] private string[] hotUpdateAssemblies;
        //private SerializedProperty baseHotUpdateAssembliesProp;
        // [SerializeField] private string[] baseHotUpdateAssemblies;

        [MenuItem("GameTool/AssetBundle/AssetBundle Packer")]
        public static void ShowWindow()
        {
            GetWindow<ABPackerWindow>("AssetBundle Packer");
        }

        private void OnEnable()
        {
            serializedObject = new SerializedObject(this);
            hotUpdateAssembliesProp = serializedObject.FindProperty(nameof(hotUpdateAssemblies));
            //baseHotUpdateAssembliesProp = serializedObject.FindProperty(nameof(baseHotUpdateAssemblies));

            outputPath = Path.Combine(Application.dataPath, "AssetBundles", EditorUserBuildSettings.activeBuildTarget.ToString());

            hotUpdateAssemblies = new[]
            {
                "HotUpdate.Common", "HotUpdate.Base","HotUpdate.Game","HotUpdate.UI","HotUpdate.Entry",
            };

            minSize = new Vector2(1389, 725);

            // 初始化各模块
            collector = new AssetBundleCollector(AssetsInputPath, filterSuffixes, filterDirectories, AppendToLog, DisplayProgress);
            differ = new AssetBundleDiffer(AppendToLog, DisplayProgress);
            dependencyResolver = new AssetBundleDependencyResolver(AppendToLog, DisplayProgress);
            builder = new AssetBundleBuilder(AppendToLog, DisplayProgress);
            uploader = new AssetBundleUploader(AppendToLog);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            leftScrollPos = EditorGUILayout.BeginScrollView(leftScrollPos);
            DrawLeftArea();
            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();

            EditorGUILayout.Space(1);

            GUILayout.BeginVertical(GUILayout.Width(position.width * 0.3f));
            DrawRightArea();
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        #region Left Area Drawing
        private void DrawLeftArea()
        {
            DrawResKeyView();
            DrawHotUpdateView();
            DrawAssetsCollectView();
            DrawContrastDifference();
            DrawLabelsView();
            DrawSaveCollectionView();
            DrawTargetSelectView();
            DrawOutputPathView();
            DrawBuildSettingsView();
            DrawDependenceAnalysisView();
            DrawCopySettingsView();
            DrawServerSettingsView();
        }

        private void DrawResKeyView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Asset Key Generate", EditorStyles.boldLabel);
    
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh Asset Keys Now", GUILayout.Width(200)))
            {
                RefreshAssetKeys();
                AppendToLog("资源键常量已手动刷新。");
            }
            GUILayout.EndHorizontal();
        }
        
        private void DrawHotUpdateView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Assembly HotUpdate", EditorStyles.boldLabel);
            EditorGUILayout.TextField("HybridCLR Dlls Path", hybridCLRAssemblySourcesPath);
            serializedObject.Update();
            EditorGUILayout.PropertyField(hotUpdateAssembliesProp, true);
            if (GUILayout.Button("Copy HotUpdate Assembly"))
                MoveHotUpdateAssembly();
            //EditorGUILayout.PropertyField(baseHotUpdateAssembliesProp, true);
            serializedObject.ApplyModifiedProperties();

            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            var savePath = $"{Path.Combine(Application.dataPath, "Editor", "ArtRes", "HotUpdate", $"{nameof(HotUpdateAssemblySettings)}.json")}";
            EditorGUILayout.TextField("Generate Path", savePath);
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("Generate Hotfix Asm Settings", GUILayout.Width(200)))
            {
                GenerateDllDependencyFile(savePath, hotUpdateAssemblies);
                AppendToLog($"HotUpdateAssemblySettings Generate At：{savePath}\n");
            }
            GUILayout.EndHorizontal();
        }

        private void DrawAssetsCollectView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Collect AssetInfos", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            assetsCollection_Temp = EditorGUILayout.ObjectField("Latest Asset Collection", assetsCollection_Temp, typeof(AssetBundlesCollections), false) as AssetBundlesCollections;
            if (GUILayout.Button("Load Collection", GUILayout.Width(120)))
            {
                assetsCollection_Temp = AssetDatabase.LoadAssetAtPath<AssetBundlesCollections>($"{abSettingsSavePath}{assetCollectionName_Temp}");
                if (!assetsCollection_Temp) AppendToLog($"'{assetCollectionName_Temp}' file do not exist, please 'Collect'\n");
            }
            if (GUILayout.Button("Collect", GUILayout.Width(120)))
            {
                assetsCollection_Temp = collector.CollectLatestInfos(abSettingsSavePath, assetCollectionName_Temp);
            }
            GUILayout.EndHorizontal();
        }
        
        private void DrawContrastDifference()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Compare Assets Difference", EditorStyles.boldLabel);
            if (GUILayout.Button("Handle Assets Difference"))
            {
                abNameToDifferenceInfos.Clear();
                waitRemoveAbNames.Clear();
                waitRemoveAssetInfos.Clear();
                forceUploadBundles.Clear();

                assetsCollection_Release = AssetDatabase.LoadAssetAtPath<AssetBundlesCollections>($"{abSettingsSavePath}{assetCollectionName_Release}");
                var result = differ.Compare(assetsCollection_Temp, assetsCollection_Release);

                abNameToDifferenceInfos = result.BundlesToRebuild;
                waitRemoveAbNames = result.BundlesToRemove;
                waitRemoveAssetInfos = result.AssetsToRemovePerBundle;

                // 扩展依赖
                // 获取上一次打包到输出路径的依赖文件
                var lastManifest = Path.Combine(outputPath, AssetBundleUtility.GetPlatformBundleName(targetPlatform));
                if (File.Exists(lastManifest))
                {
                    dependencyResolver.ExpandWithDependencies(abNameToDifferenceInfos, waitRemoveAbNames, assetsCollection_Release, lastManifest, forceUploadBundles);
                }
            }
        }

        private void DrawLabelsView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Label Settings", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Set Labels"))
                collector.SetLabels(abNameToDifferenceInfos, assetsCollection_Release, waitRemoveAbNames);
            if (GUILayout.Button("Clear All Labels"))
                collector.ClearAllLabels();
            GUILayout.EndHorizontal();
        }
        
        private void DrawSaveCollectionView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Update Collection", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            assetsCollection_Release = EditorGUILayout.ObjectField("Asset Collection(Release)", assetsCollection_Release, typeof(AssetBundlesCollections), false) as AssetBundlesCollections;
            if (GUILayout.Button("Override Collection", GUILayout.Width(150)))
            {
                if (EditorUtility.DisplayDialog("Override Collection", "你确定要覆盖当前的资源集合吗？", "确定"))
                {
                    if (AssetDatabase.CopyAsset($"{abSettingsSavePath}{assetCollectionName_Temp}", $"{abSettingsSavePath}{assetCollectionName_Release}"))
                    {
                        assetsCollection_Release = AssetDatabase.LoadAssetAtPath<AssetBundlesCollections>($"{abSettingsSavePath}{assetCollectionName_Release}");
                        EditorUtility.SetDirty(assetsCollection_Release);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        AppendToLog("Override Success!");

                        // ========== 已移除 AbKeyCollection 脚本生成 ==========
                        // 因为现在采用基于资源名的加载方式，不再需要 AB 包常量类
                        // ===================================================
                        
                        // var abNames = assetsCollection_Release.assetBundleInfos.ConvertAll(ab => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ab.assetBundleName));
                        // new AbKeyCollectionClassGenerator(abNames).GenerateScript();
                        // AssetDatabase.Refresh();
                        // AppendToLog("Generated Script：AbKeyCollection\n");
                    }
                    else AppendToLog("Failed to handle the differences");
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawTargetSelectView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Build Settings", EditorStyles.boldLabel);
            targetPlatform = (BuildTarget)EditorGUILayout.EnumPopup("Target Platform", targetPlatform);
        }

        private void DrawOutputPathView()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            outputPath = EditorGUILayout.TextField("Output Path", outputPath);
            if (GUILayout.Button("Browse...", GUILayout.Width(80)))
            {
                string path = EditorUtility.OpenFolderPanel("Select Output Folder", Path.GetDirectoryName(outputPath), "");
                if (!string.IsNullOrEmpty(path)) outputPath = path;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawBuildSettingsView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Build Options", EditorStyles.boldLabel);
            buildOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField("Options", buildOptions);
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Build AssetBundles"))
            {
                builder.Build(outputPath, targetPlatform, buildOptions, AssetsInputPath, assetsCollection_Release);
            }
            if (GUILayout.Button("Clean Output"))
            {
                builder.CleanOutputDirectory(outputPath);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawDependenceAnalysisView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Dependency Analysis", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            mainBundlePath = EditorGUILayout.TextField("Main AB Path", mainBundlePath);
            if (GUILayout.Button("Load Main", GUILayout.Width(120)))
            {
                string path = EditorUtility.OpenFilePanel("Select MainBundle", "", "assetBundle");
                if (!string.IsNullOrEmpty(path)) mainBundlePath = path;
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Analyze Dependencies"))
            {
                dependencyResolver.AnalyzeDependencies(mainBundlePath, targetPlatform);
                //string listPath = Path.Combine(outputPath, FileUtility.ListFileDefaultName);
                //dependencyResolver.CreateListFile(outputPath, listPath, mainBundlePath, targetPlatform, jsonManager);
            }
        }

        private void DrawCopySettingsView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("AssetBundle Copy", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("ServerData Path", serverDataPath);
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("Update AssetBundle And ListFile In ServerData", GUILayout.Width(300)))
                builder.CopyToServerData(outputPath, serverDataPath, assetsCollection_Release, targetPlatform);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("StreamingAssets Path", AB_COPY_PATH);
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("Move AssetBundle To StreamingAssets", GUILayout.Width(300)))
                builder.MoveToStreamingAssets(AB_COPY_PATH, outputPath, Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets));
            GUILayout.EndHorizontal();
        }

        private void DrawServerSettingsView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Server Settings", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            serverIP = EditorGUILayout.TextField("Server Path", serverIP);
            if (GUILayout.Button("Load From Global Settings", GUILayout.Width(250)))
                serverIP = GlobalSettings.Instance.updateModuleConfig.resServerIp;
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            uploadUseUser = GUILayout.Toggle(uploadUseUser, "Verify Identity");
            if (uploadUseUser)
            {
                EditorGUILayout.Space();
                userName = EditorGUILayout.TextField("UserName", userName);
                GUILayout.BeginHorizontal();
                password = showPassWord ? EditorGUILayout.TextField("Password", password) : EditorGUILayout.PasswordField("Password", password);
                showPassWord = GUILayout.Toggle(showPassWord, "Show Password");
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
            GUILayout.Label("Max UpLoad-Bytes Per One Time");
            GUILayout.BeginHorizontal();
            uploadBytesIsCustomSetting = GUILayout.Toggle(!uploadBytesIsAutoSetting, "Custom", GUILayout.Width(80));
            uploadBytesIsAutoSetting = GUILayout.Toggle(!uploadBytesIsCustomSetting, "Auto", GUILayout.Width(80));
            GUILayout.EndHorizontal();
            if (!uploadBytesIsAutoSetting)
            {
                EditorGUILayout.Space();
                maxBytesCapacity = uint.Parse(GUILayout.TextField(maxBytesCapacity.ToString()), NumberStyles.Number);
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Upload AssetBundleData"))
            {
                uploader.UploadIncrementalAsync(serverDataPath, serverIP,
                    uploadUseUser, userName, password, uploadBytesIsAutoSetting, 
                    maxBytesCapacity, AssetBundleBuilder.AssetCatalogName, forceUploadBundles);
            }
        }
        #endregion

        #region Right Area Drawing
        private void DrawRightArea()
        {
            DrawLogView();
            DrawUtilityView();
        }

        private void DrawLogView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Build Log", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.TextArea(buildLog, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        private void DrawUtilityView()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Open Output Directory")) EditorUtility.RevealInFinder(outputPath);
            if (GUILayout.Button("Open ServerData Directory")) EditorUtility.RevealInFinder(serverDataPath);
            if (GUILayout.Button("Clear Log")) buildLog = "";
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Helper Methods
        private void AppendToLog(string message)
        {
            buildLog += $"{message}\n";
            scrollPos.y = float.MaxValue;
        }

        private void DisplayProgress(string msg, float val)
        {
            EditorUtility.DisplayProgressBar("Processing", msg, val);
        }

        private void MoveHotUpdateAssembly()
        {
            if (!Directory.Exists(hybridCLRAssemblySourcesPath))
            {
                AppendToLog($"路径：{hybridCLRAssemblySourcesPath}不存在，请先生成热更程序集");
                return;
            }
            var srcDir = new DirectoryInfo(hybridCLRAssemblySourcesPath);
            var targetDir = new DirectoryInfo(hotUpdateAssemblyTargetPath);
            foreach (var file in targetDir.GetFiles()) file.Delete();
            AppendToLog("--- Copy HotUpdate Dlls ---");
            foreach (var file in srcDir.GetFiles())
            {
                if (file.Extension == ".dll" && file.Name.Contains("HotUpdate"))
                {
                    foreach (var asm in hotUpdateAssemblies)
                    {
                        if (file.Name.Contains(asm))
                        {
                            File.Copy(file.FullName, Path.Combine(hotUpdateAssemblyTargetPath, file.Name + ".bytes"), true);
                            AppendToLog($"Copy Over：{hotUpdateAssemblyTargetPath}{file.Name}.bytes");
                            break;
                        }
                    }
                }
            }
            AppendToLog("--- Copy HotUpdate Dlls End ---\n");
            AssetDatabase.Refresh();
        }

        private void GenerateDllDependencyFile(string savePath, string[] hotUpdateAssemblies)
        {
            // 获取当前目标平台的所有程序集（如果需要在构建时区分平台）
            var allAssemblies = CompilationPipeline.GetAssemblies();
            var allDlls = allAssemblies.ToDictionary(a => a.name);

            var settings = new HotUpdateAssemblySettings();
            var processed = new HashSet<string>();
            var queue = new Queue<string>(hotUpdateAssemblies);

            // 需要过滤的引擎程序集前缀（可根据实际调整）
            var ignorePrefixes = new[]
            {
                "Unity", "UnityEngine", "UnityEditor", "System", "mscorlib", "netstandard",
                "CoreModule", "GameModule"
            };

            while (queue.Count > 0)
            {
                var name = queue.Dequeue();
                if (!processed.Add(name)) continue;
                if (!allDlls.TryGetValue(name, out var assembly)) continue;

                // 获取直接引用，并过滤掉引擎程序集
                var deps = assembly.assemblyReferences
                    .Select(d => d.name + ".dll")   // 直接加后缀
                    .Where(d => !ignorePrefixes.Any(d.StartsWith)) // 注意 lambda 别遮蔽
                    .ToList();

                settings.dllDependencies[$"{name}.dll"] = deps;
                AppendToLog($"{name} -> 依赖: {string.Join(',', deps)}");
                
                // 因为手动将所有可能用到的程序集都添加为了直接引用，所以这个foreach可以不要，但是这并非通用做法，所以保留自动计算传递依赖
                foreach (var dep in deps)
                    queue.Enqueue(dep);
            }

            jsonManager.SaveToJson(settings, savePath);
            AssetDatabase.Refresh();
        }
        
        /// <summary>
        /// 从临时收集的资源快照中生成资源键常量（不覆盖 Release 快照）
        /// </summary>
        public static void RefreshAssetKeys()
        {
            var collector = new AssetBundleCollector(
                AssetsInputPath, 
                new[] { ".meta" }, 
                null, 
                null, 
                null
            );
    
            // 使用一个不会影响发布配置的临时文件名
            const string tempFileName = "AssetBundlesCollections_Temp_Keys.asset";
            var savePath = "Assets/Editor/AssetBundleSettings/";
    
            var tempCollection = collector.CollectLatestInfos(savePath, tempFileName);
            if (!tempCollection)
            {
                Debug.LogError("刷新资源键失败：无法收集资源信息。");
                return;
            }

            // 生成资源键常量
            var keys = new HashSet<string>();
            foreach (var abInfo in tempCollection.assetBundleInfos)
            {
                foreach (var assetInfo in abInfo.assetInfos)
                {
                    var key = Path.GetFileNameWithoutExtension(assetInfo.name);
                    keys.Add(key);
                }
            }

            var assetKeyScriptPath = Path.Combine(Application.dataPath, "Scripts", "HotUpdate", "Common", "Generated", "AssetKeys.cs");
            AssetKeyGenerator.GenerateFromKeys(keys, assetKeyScriptPath);
    
            // 生成 AB 包键常量
            var bundleNames = tempCollection.assetBundleInfos.Select(ab => ab.assetBundleName).ToList();
            var bundleKeyScriptPath = Path.Combine(Application.dataPath, "Scripts", "HotUpdate", "Common", "Generated", "AssetBundleKeys.cs");
            AssetBundleKeyGenerator.GenerateFromNames(bundleNames, bundleKeyScriptPath);
    
            // 清理临时文件（可选）
            AssetDatabase.DeleteAsset($"{savePath}{tempFileName}");
            AssetDatabase.Refresh();
    
            Debug.Log($"资源键常量已刷新。\nAssetKeys: {assetKeyScriptPath}\nAssetBundleKeys: {bundleKeyScriptPath}");
        }
        #endregion
    }
}