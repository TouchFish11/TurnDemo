using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Core.AssetBundles.Update.Collection;
using Core.DataPersistence.Json;
using Core.Global;
using Core.Utility;
using Editor.Generation.Detail;
using Editor.Menu;
using Framework.Editor.Generation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.AssetBundle
{
    // 自定义 Build 时的回调，用于在构建前后执行操作
    public class ABPackerBuildHook : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            // 可以在这里添加构建前的准备工作
            Debug.Log($"[ABPackerHook] Starting build for target: {report.summary.platform}");
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            // 可以在这里添加构建后的清理或通知工作
            Debug.Log($"[ABPackerHook] Build finished for target: {report.summary.platform}, Duration: {report.summary.totalTime.TotalSeconds:F2}s");
        }
    }
    
    public class ABPackerWindow : EditorWindow
    {
        // --- GUI 状态 ---
        private Vector2 _scrollPos = Vector2.zero;
        private string _buildLog = "";

        // --- 配置参数 ---
        private BuildTarget _targetPlatform = BuildTarget.StandaloneWindows64; // 默认平台
        private BuildAssetBundleOptions _buildOptions = BuildAssetBundleOptions.None; // 构建选项
        private string _AssemblyhotUpdateTargetPath;  // 热更新程序集目标路径
        private string _outputPath; // 输出路径
        private const string AB_COPY_PATH = "Assets/StreamingAssets/AssetBundles/";     // AssetBundle拷贝到StreamingAssets的目标路径
        private string _mainBundlePath = "";  // 用于依赖分析的主包路径
        private string serverIP = "http://...";     // 资源上传服务器地址
        private bool uploadUseUser;     // 上传时是否启用用户身份验证
        private string userName = "userName";   // 上传服务器的用户名
        private string password = "password";   // 上传服务器的密码
        private bool showPassWord;  // 是否显示明文密码
        private bool uploadBytesIsAutoSetting = true;   // 上传字节数是否自动配置
        private bool uploadBytesIsCustomSetting;    // 上传字节数是否自定义配置（与auto互斥）
        private uint maxBytesCapacity = 4096;   // 单次上传的最大字节数（自定义模式下生效）
        private static int upLoadmaxNum;    // 待上传文件总数
        private static int nowUpLoadFinishedNum;    // 已完成上传的文件数
        private const string targetPath = @"D:\UnityProject\TurnDemo\Assets\Editor\ArtRes\HotUpdate\Assembly-CSharp-Game-HotUpdate.dll.bytes";
        private const string AssetsInputPath = "Assets/Editor/ArtRes/";     // 待打包资源的输入根路径
        private readonly Dictionary<string, List<FileInfo>> _fileInfoDic = new();   // 存储待处理文件信息的字典：Key为目录名，Value为该目录下的文件列表
        private readonly string[] filterDirectorys = { "Texture" };     // 过滤的文件夹
        private readonly string[] _filterSuffixes = { ".meta" };    // 需要过滤的文件后缀（打包时忽略）
        private const string _abSettingsSavePath = "Assets/Editor/AssetBundleSettings/"; // AB包SO设置保存路径
        private readonly Dictionary<string, List<AssetBundlesCollections.AssetInfo>> abNameToDifferenceInfos = new();   // 记录差异，之后只设置这些资源标签
        private readonly List<string> waitRemoveAbNames = new();    // 待移除的AB包名称
        private readonly Dictionary<string, List<AssetBundlesCollections.AssetInfo>> waitRemoveAssetInfos = new();  // 待移除的资源
        private AssetBundlesCollections _assetsColletion_Temp;
        private const string _assetCollectionName_Temp = "AssetBundlesCollections_Temp.asset";
        private AssetBundlesCollections _assetsCollection_Release;
        private const string _assetCollectionName_Release = "AssetBundlesCollections.asset";
        private readonly string serverDataPath = $"{Application.dataPath}/ServerData/";     // 服务器数据编辑器路径
        
        [MenuItem("Tools/AssetBundle Packer")]
        public static void ShowWindow()
        {
            GetWindow<ABPackerWindow>("AssetBundle Packer");
        }

        private void OnEnable()
        {
            // 初始化时设置默认输出路径
            _outputPath = Path.Combine(Application.dataPath, "AssetBundles", EditorUserBuildSettings.activeBuildTarget.ToString());
            _AssemblyhotUpdateTargetPath = targetPath;
            minSize = new Vector2(1389, 725);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            
            // 左侧布局
            GUILayout.BeginVertical();
            OnDrawLeftArea();
            GUILayout.EndVertical(); 
            
            EditorGUILayout.Space(1);
            
            // 右侧布局
            GUILayout.BeginVertical(GUILayout.Width(position.width * 0.3f)); 
            OnDrawRightArea();
            GUILayout.EndVertical(); 
            
            EditorGUILayout.EndHorizontal();
        }

        #region 左侧区域视图
        private void OnDrawLeftArea()
        {
            // --- AB包构建配置 ---
            DrawConfigSettingsView();

            // --- 热更新 ---
            DrawHotUpdateView();
            
            // --- 收集最新资源信息 ---
            DrawAssetsCollectView();
            
            // --- 对比新旧资源差异 ---
            DrawContrastDifference();
            
            // --- AB包标签 ---
            DrawLabelsView();
            
            // --- 保存AB包配置 ---
            DrawSaveCollectionView();
            
            // --- 平台选择 ---
            DrawTargetSelectView();

            // --- 输出路径 ---
            DrawOutputPathView();

            // --- 构建选项 ---
            DrawBuildSettingsView();
            
            // --- 依赖分析 ---
            DrawDependenceyAnalysisView();

            // --- AB包清单 ---
            DrawListSettingsView();

            // --- AB包拷贝 ---
            DrawCopySettingsView();
            
            // --- 服务器 ---
            DrawServerSettingsView();
        }

        private void DrawServerSettingsView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Server Settings", EditorStyles.boldLabel);
            
            // 服务器地址配置
            GUILayout.BeginHorizontal();
            serverIP = EditorGUILayout.TextField("Server Path", serverIP);
            if (GUILayout.Button("Load From Global Settings", GUILayout.Width(250)))
            {
                serverIP = GlobalSettings.Instance.resServerIp;
            }
            GUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            // 上传身份验证开关
            uploadUseUser = GUILayout.Toggle(uploadUseUser, new GUIContent("Verify Identity", "上传时启用用户名/密码身份验证"));
            // 身份验证配置（仅当开关开启时显示）
            if (uploadUseUser)
            {
                EditorGUILayout.Space();
                userName = EditorGUILayout.TextField("UserName",userName);
                
                GUILayout.BeginHorizontal();
                password = showPassWord ? EditorGUILayout.TextField("Password",password) : EditorGUILayout.PasswordField("Password", password);
                showPassWord = GUILayout.Toggle(showPassWord, "Show Password");
                
                GUILayout.EndHorizontal();
            }
            
            // 上传字节数配置
            EditorGUILayout.Space();
            GUILayout.Label(new GUIContent("Max UpLoad-Bytes Per One Time", "单次上传的最大字节数配置"));
            
            GUILayout.BeginHorizontal();
            uploadBytesIsCustomSetting = GUILayout.Toggle(!uploadBytesIsAutoSetting, new GUIContent("Custom", "自定义配置"), GUILayout.Width(80));
            uploadBytesIsAutoSetting = GUILayout.Toggle(!uploadBytesIsCustomSetting, new GUIContent("Auto", "自动配置（按文件大小分级）"), GUILayout.Width(80));
            GUILayout.EndHorizontal();
            
            // 自定义上传字节数输入（仅当自定义模式开启时显示）
            if (!uploadBytesIsAutoSetting)
            {
                EditorGUILayout.Space();
                GUILayout.Label(new GUIContent("Custom Max UpLoad-Bytes Per One Time", "自定义单次上传最大字节数"));
                maxBytesCapacity = uint.Parse(GUILayout.TextField(maxBytesCapacity.ToString()), NumberStyles.Number);
            }

            EditorGUILayout.Space();
            bool isClick;
            if (upLoadmaxNum == nowUpLoadFinishedNum)
            {
                isClick = GUILayout.Button(new GUIContent("Upload AssetBundleDatas", "上传当前平台的AB包到服务器"));
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                isClick = GUILayout.Button(new GUIContent("Upload AssetBundleDatas", "上传当前平台的AB包到服务器"));
                EditorGUI.EndDisabledGroup();
            }
            
            // 上传AB包按钮
            if (isClick)
            {
                UpLoadAssetBundlesData();
            }
        }

        private void DrawDependenceyAnalysisView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Dependency Analysis", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            _mainBundlePath = EditorGUILayout.TextField("Main AB Path", _mainBundlePath);
            if (GUILayout.Button("Load Main", GUILayout.Width(120)))
            {
                var selectedPath = EditorUtility.OpenFilePanel("Select MainBunlde", "", "assetBundle");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    _mainBundlePath = selectedPath;
                }
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Analyze Dependencies And Create Temp ABListFile"))
            {
                // 分析依赖
                AnalyzeDependencies();
                
                // 创建临时AB包清单文件
                CreateAssetBundleListFile(_outputPath, $"{_outputPath}/{FileUtility.ListFileDefaultName}");
            }
        }

        private void DrawCopySettingsView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("AssetBundle Copy", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Copy Path", AB_COPY_PATH);
            EditorGUI.EndDisabledGroup();
            
            // 拷贝AB包到StreamingAssets按钮
            if (GUILayout.Button("Move AssetBundle To StreamingAssets",  GUILayout.Width(300)))
            {
                MoveABToStreamingAssets();
            }
            
            GUILayout.EndHorizontal();
        }

        private void DrawListSettingsView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Asset List", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("List Path", $"{serverDataPath}AssetBundleListInfo.json");
            EditorGUI.EndDisabledGroup();
            
            // 生成AB包清单文件按钮
            if (GUILayout.Button("Create ABListFile", GUILayout.Width(250)))
            {
                CreateAssetBundleListFile
                (
                    serverDataPath, 
                    $"{serverDataPath}{FileUtility.ListFileDefaultName}"
                );
            }
            
            GUILayout.EndHorizontal();
        }

        private void DrawBuildSettingsView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Build Options", EditorStyles.boldLabel);
            
            _buildOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField("Options", _buildOptions);

            // --- 构建按钮 ---
            EditorGUILayout.Space();
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Build AssetBundles"))
            {
                BuildAssetBundles();
            }
            if (GUILayout.Button("Copy To ServerData"))
            {
                CopyToServerData();
            }
            if (GUILayout.Button("Clean Output"))
            {
                CleanOutputDirectory();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawOutputPathView()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            
            _outputPath = EditorGUILayout.TextField("Output Path", _outputPath);
            if (GUILayout.Button("Browse...", GUILayout.Width(80)))
            {
                var selectedPath = EditorUtility.OpenFolderPanel("Select Output Folder", Path.GetDirectoryName(_outputPath), string.Empty);
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    _outputPath = selectedPath;
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawTargetSelectView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Build Settings", EditorStyles.boldLabel);
            
            _targetPlatform = (BuildTarget)EditorGUILayout.EnumPopup("Target Platform", _targetPlatform);
        }

        private void DrawSaveCollectionView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Update Collection", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            _assetsCollection_Release = EditorGUILayout.ObjectField("Asset Collection(Realse)", _assetsCollection_Release, typeof(AssetBundlesCollections), false) as AssetBundlesCollections;
            if (GUILayout.Button("Override Collection",  GUILayout.Width(150)))
            {
                var isOk = EditorUtility.DisplayDialog("Override Collection", "你确定要覆盖当前的资源集合吗？", "确定");
                if (isOk)
                {
                    if (AssetDatabase.CopyAsset($"{_abSettingsSavePath}{_assetCollectionName_Temp}", $"{_abSettingsSavePath}{_assetCollectionName_Release}"))
                    {
                        //_assetsCollection_Release = AssetDatabase.LoadAssetAtPath<AssetBundlesCollections>($"{_abSettingsSavePath}{_assetCollectionName_Release}");
                        // // 移除无用的AB包
                        // foreach (var waitRemoveAbName in waitRemoveAbNames)
                        // {
                        //     var index = _assetsCollection_Release.assetBundleInfos.FindIndex(abInfo => abInfo.assetBundleName == waitRemoveAbName);
                        //     if (index != -1)
                        //     {
                        //         _assetsCollection_Release.assetBundleInfos.RemoveAt(index);
                        //     }
                        // }
                        
                        // // 移除无用的资源
                        // foreach (var abName in waitRemoveAssetInfos.Keys)
                        // {
                        //     var assstInfos = waitRemoveAssetInfos[abName];
                        //     var abInfo = _assetsCollection_Release.assetBundleInfos.Find(abInfo => abInfo.assetBundleName == abName);
                        //     if (abInfo != null)
                        //     {
                        //         foreach (var assstInfo in assstInfos)
                        //         {
                        //             var index = abInfo.assetInfos.FindIndex(info => info.name == assstInfo.name);
                        //             if (index != -1)
                        //             {
                        //                 abInfo.assetInfos.RemoveAt(index);
                        //             }
                        //         }
                        //     }
                        // }
                        
                        // foreach (var abName in abNameToDifferenceInfos.Keys)
                        // {
                        //     var abInfo = _assetsCollection_Release.assetBundleInfos.Find(abInfo => abInfo.assetBundleName == abName);
                        //     // 存在该AB包
                        //     if (abInfo != null)
                        //     {
                        //         var assetInfos = abNameToDifferenceInfos[abName];
                        //         abInfo.assetInfos.AddRange(assetInfos);
                        //     }
                        //     // 不存在AB包
                        //     else
                        //     {
                        //         var assetInfos = abNameToDifferenceInfos[abName];
                        //         foreach (var assetInfo in assetInfos)
                        //         {
                        //             _assetsCollection_Release.Add(abName,  assetInfo);
                        //         }
                        //     }
                        // }
                    
                        EditorUtility.SetDirty(_assetsCollection_Release);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        
                        AppendToLog($"Override Success!");
                        
                        // 生成脚本
                        var textInfo = CultureInfo.CurrentCulture.TextInfo;
                        var abNames = _assetsCollection_Release.assetBundleInfos.ConvertAll(abInfo => textInfo.ToTitleCase(abInfo.assetBundleName));
                        // 生成AssetBundle类型键脚本
                        IScriptGenerator scriptGenerator = new AbKeyCollectionClassGenerator(abNames);
                        scriptGenerator.GenerateScript();
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        AppendToLog($"Failed to handle the differences");
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawLabelsView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Label Settings", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            // 设置AB包标签按钮
            if (GUILayout.Button("Set Labels"))
            {
                EditAssetLabel();
            }
            
            // 清空AB包标签按钮
            if (GUILayout.Button("Clear All Labels"))
            {
                ClearAssetLabel();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawContrastDifference()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Compare Assets Difference", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Handle Assets Difference"))
            {
                // 先清空缓存
                abNameToDifferenceInfos.Clear();
                waitRemoveAbNames.Clear();
                waitRemoveAssetInfos.Clear();
                
                // 尝试读取
                _assetsCollection_Release = AssetDatabase.LoadAssetAtPath<AssetBundlesCollections>($"{_abSettingsSavePath}{_assetCollectionName_Release}");
                if (!_assetsCollection_Release)
                {
                    // 说明是第一次，则直接把最新的全部添加到abNameToDifferenceInfos，因为第一次是全量打包，不存在差异
                    foreach (var assetBundleInfo in _assetsColletion_Temp.assetBundleInfos)
                    {
                        abNameToDifferenceInfos.Add(assetBundleInfo.assetBundleName, assetBundleInfo.assetInfos);
                    }
                    
                    AppendToLog($"Check First comparison Difference, will full build");
                }
                else
                {
                    // 移除发布版配置无用的包
                    for (var i = _assetsCollection_Release.assetBundleInfos.Count - 1; i >= 0; i--)
                    {
                        var abInfo_Realese = _assetsCollection_Release.assetBundleInfos[i];
                        var index = _assetsColletion_Temp.assetBundleInfos.FindIndex(abInfo => abInfo_Realese.assetBundleName == abInfo.assetBundleName);
                        // 发布版配置中的包在最新版配置中没有找到，说明不需要这个包，需要在发布版中移除这个包
                        if (index == -1)
                        {
                            waitRemoveAbNames.Add(abInfo_Realese.assetBundleName);
                            AppendToLog($"Found Unuesed Bundle：{abInfo_Realese.assetBundleName}");
                        }
                    }
                    
                    // 移除发布版配置无用的资源
                    for (var i = _assetsCollection_Release.assetBundleInfos.Count - 1; i >= 0; i--)
                    {
                        var abInfo_Release = _assetsCollection_Release.assetBundleInfos[i];
                        var abInfo_Temp = _assetsColletion_Temp.assetBundleInfos.Find(abInfo => abInfo.assetBundleName == abInfo_Release.assetBundleName);
                        if (abInfo_Temp != null)
                        {
                            for (var j = abInfo_Release.assetInfos.Count - 1; j >= 0; j--)
                            {
                                var assetInfo_Release =  abInfo_Release.assetInfos[j];
                                // 判断旧发布版配置中的资源在最新配置中是否存在，不存在则删除发布版本的该资源
                                var index = abInfo_Temp.assetInfos.FindIndex(assetInfo => assetInfo.name == assetInfo_Release.name);
                                if (index == -1)
                                {
                                    if (waitRemoveAssetInfos.ContainsKey(abInfo_Release.assetBundleName))
                                    {
                                        waitRemoveAssetInfos[abInfo_Release.assetBundleName].Add(assetInfo_Release);
                                    }
                                    else
                                    {
                                        waitRemoveAssetInfos.Add(abInfo_Release.assetBundleName, new List<AssetBundlesCollections.AssetInfo> { assetInfo_Release });
                                    }
                                    
                                    AppendToLog($"Found Unuesed Asset：{assetInfo_Release.name} in '{abInfo_Release.assetBundleName}'");
                                }
                            }
                        }
                    }
                    
                    // 对比差异
                    for (var i = 0; i < _assetsColletion_Temp.assetBundleInfos.Count; i++)
                    {
                        // 获取单个包信息
                        var assetBundleInfo_Temp = _assetsColletion_Temp.assetBundleInfos[i];
                        var abInfo_Realese = _assetsCollection_Release.assetBundleInfos.Find(abInfo => abInfo.assetBundleName == assetBundleInfo_Temp.assetBundleName);
                        // 没有这个包
                        if (abInfo_Realese == null)
                        {
                            // 发布版的配置需要新增这个包信息，包含所有资源信息
                            //_assetsCollection_Release.assetBundleInfos.Add(assetBundleInfo_Temp);
                            // 加入字典，这个包的所有资源都要被打包
                            abNameToDifferenceInfos.Add(assetBundleInfo_Temp.assetBundleName, assetBundleInfo_Temp.assetInfos);
                            
                            var assetNames = assetBundleInfo_Temp.assetInfos.ConvertAll(info => info.name);
                            AppendToLog($"Add NewBundle：{assetBundleInfo_Temp.assetBundleName}，Include Assets：[{string.Join(',', assetNames)}]");
                        }
                        // 发布版的配置有这个包
                        else
                        {
                            // 判断有没有变化的资源
                            for (var k = 0; k < assetBundleInfo_Temp.assetInfos.Count; k++)
                            {
                                var assetInfo_Temp = assetBundleInfo_Temp.assetInfos[k];
                                // 发布包是否存在该资源
                                var assetInfo_Realese = abInfo_Realese.assetInfos.Find(abInfo => abInfo.name == assetInfo_Temp.name);
                                // 不存在，直接添加
                                if (assetInfo_Realese == null)
                                {
                                    // 把该资源添加到发布版配置中
                                    //abInfo_Realese.assetInfos.Add(assetInfo_Temp);
                                    // 添加到字典中
                                    // 若字典存在这个包名，说明之前已经添加了这个包的所有资源了，只需添加这个资源即可，避免重复添加
                                    if (abNameToDifferenceInfos.ContainsKey(assetBundleInfo_Temp.assetBundleName))
                                    {
                                        abNameToDifferenceInfos[assetBundleInfo_Temp.assetBundleName].Add(assetInfo_Temp);
                                        AppendToLog($"Add NewAsset：{assetInfo_Temp.name} at {assetBundleInfo_Temp.assetBundleName} bundle");
                                    }
                                    // 若不存在于字典，说明是第一次处理该包，把这个包的所有资源添加进去
                                    else
                                    {
                                        abNameToDifferenceInfos.Add(assetBundleInfo_Temp.assetBundleName, abInfo_Realese.assetInfos);
                                        
                                        var assetNames = abInfo_Realese.assetInfos.ConvertAll(info => info.name);
                                        AppendToLog($"Rebuild Bundle：{assetBundleInfo_Temp.assetBundleName}，Include Assets：[{string.Join(',', assetNames)}]");
                                    }
                                }
                                // 存在判断是否相等
                                else
                                {
                                    // 全部相等，则资源没有变化，不用重新打包
                                    if (assetInfo_Realese.hash == assetInfo_Temp.hash && 
                                        assetInfo_Realese.name == assetInfo_Temp.name && 
                                        assetInfo_Realese.size == assetInfo_Temp.size && 
                                        assetInfo_Realese.assetPath == assetInfo_Temp.assetPath)
                                    {
                                        continue;
                                    }
                                    
                                    // 资源名称和hash相等，不用重新打包，但是路径不相同，说明资源位置发生变化，只需更新路径
                                    if (assetInfo_Realese.hash == assetInfo_Temp.hash && 
                                        assetInfo_Realese.name == assetInfo_Temp.name && 
                                        assetInfo_Realese.size == assetInfo_Temp.size && 
                                        assetInfo_Realese.assetPath != assetInfo_Temp.assetPath)
                                    {
                                        assetInfo_Realese.assetPath = assetInfo_Temp.assetPath;
                                        AppendToLog($"Update AssetPath：{assetInfo_Temp.name} at {assetBundleInfo_Temp.assetBundleName} bundle");
                                    }
                                    // 其它情况都要打包
                                    else
                                    {
                                        // 先将旧资源信息放入待移除容器，只处理名称相同的资源，名称不同的资源已在[移除发布版配置无用的资源]中排除
                                        if (waitRemoveAssetInfos.ContainsKey(abInfo_Realese.assetBundleName))
                                        {
                                            waitRemoveAssetInfos[abInfo_Realese.assetBundleName].Add(assetInfo_Realese);
                                        }
                                        else
                                        {
                                            waitRemoveAssetInfos.Add(abInfo_Realese.assetBundleName, new List<AssetBundlesCollections.AssetInfo> { assetInfo_Realese });
                                        }
                                        
                                        // 字典不存在该包，说明第一次添加该包的资源，也要添加这个包的所有资源进去
                                        if (!abNameToDifferenceInfos.ContainsKey(abInfo_Realese.assetBundleName))
                                        {
                                            var newInfos =  new List<AssetBundlesCollections.AssetInfo>();
                                            // 添加新资源信息
                                            newInfos.Add(new AssetBundlesCollections.AssetInfo(assetInfo_Temp.assetPath, assetInfo_Temp.size, assetInfo_Temp.name, assetInfo_Temp.hash));
                                            foreach (var assetInfo in abInfo_Realese.assetInfos)
                                            {
                                                newInfos.Add(new AssetBundlesCollections.AssetInfo(assetInfo.assetPath, assetInfo.size, assetInfo.name,  assetInfo.hash));
                                            }
                                            
                                            abNameToDifferenceInfos.Add(abInfo_Realese.assetBundleName, newInfos);
                                            
                                            // 记录日志
                                            var assetNames = abInfo_Realese.assetInfos.ConvertAll(info => info.name);
                                            AppendToLog($"Rebuild Bundle：{abInfo_Realese.assetBundleName}，Include Assets：[{string.Join(',', assetNames)}]");
                                        }
                                        // 存在该包，添加新资源信息
                                        else
                                        {
                                            var newInfo = new AssetBundlesCollections.AssetInfo(assetInfo_Temp.assetPath, assetInfo_Temp.size, assetInfo_Temp.name, assetInfo_Temp.hash);
                                            abNameToDifferenceInfos[abInfo_Realese.assetBundleName].Add(newInfo);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (abNameToDifferenceInfos.Count == 0 && waitRemoveAbNames.Count == 0 && waitRemoveAssetInfos.Count == 0)
                    {
                        AppendToLog($"No Differences");
                    }
                }
            }
        }
         
        private void DrawAssetsCollectView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Collect AssetInfos", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            _assetsColletion_Temp = EditorGUILayout.ObjectField("Latest Asset Collection", _assetsColletion_Temp, typeof(AssetBundlesCollections), false) as AssetBundlesCollections;
            if (GUILayout.Button("Load Collection", GUILayout.Width(120)))
            {
                _assetsColletion_Temp = AssetDatabase.LoadAssetAtPath<AssetBundlesCollections>($"{_abSettingsSavePath}{_assetCollectionName_Temp}");
                if (!_assetsColletion_Temp)
                {
                    AppendToLog($"{_assetCollectionName_Temp} file do not exist, pleas 'Collect'");
                }
            }
            
            if (GUILayout.Button("Collect", GUILayout.Width(120)))
            {
                CollectAssetInfos();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawHotUpdateView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Assembly HotUpdate", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            
            _AssemblyhotUpdateTargetPath = EditorGUILayout.TextField("Target Path", _AssemblyhotUpdateTargetPath, GUILayout.ExpandWidth(true));
            
            // 拷贝热更新程序集
            if (GUILayout.Button("Copy HotUpdate Assembly", GUILayout.Width(200)))
            {
                MoveHotUpdateAssembly();
            }
            
            GUILayout.EndHorizontal();
        }
        
        private void DrawConfigSettingsView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Config Settings", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Config Path", $"{_abSettingsSavePath}AssetBundlesCollections.asset");
            EditorGUI.EndDisabledGroup();
            
            // 创建AB包配置文件按钮
            if (GUILayout.Button("Create Config", GUILayout.Width(120)))
            {
                CreateCollection(_abSettingsSavePath, "AssetBundlesCollections.asset");
            }
            
            GUILayout.EndHorizontal();
        }
        #endregion

        #region 右侧区域视图
        private void OnDrawRightArea()
        {
            // --- 日志输出 ---
            DrawLogView();

            // --- 实用工具 ---
            DrawUtilityView();
        }

        private void DrawUtilityView()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Open Output Directory"))
            {
                OpenOutputDirectory();
            }
            if (GUILayout.Button("Clear Log"))
            {
                _buildLog = "";
            }
            GUILayout.EndHorizontal();
        }

        private void DrawLogView()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Build Log", EditorStyles.boldLabel);
            
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.TextArea(_buildLog, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        #endregion

        private void CollectAssetInfos()
        {
            _assetsColletion_Temp = AssetDatabase.LoadAssetAtPath<AssetBundlesCollections>($"{_abSettingsSavePath}{_assetCollectionName_Temp}");
            if (!_assetsColletion_Temp)
            {
                _assetsColletion_Temp = CreateCollection(_abSettingsSavePath, _assetCollectionName_Temp);
            }
            else
            {
                _assetsColletion_Temp.Clear();
            }
            
            // 检查资源输入路径是否存在，不存在则创建并提示
            if (!Directory.Exists(AssetsInputPath))
            {
                Debug.Log($"资源输入路径不存在：{AssetsInputPath}，已自动创建，请放入待打包资源后重试");
                Directory.CreateDirectory(AssetsInputPath);
                return;
            }
            
            // 初始化目录信息
            var directoryInfo = Directory.CreateDirectory(AssetsInputPath);
            _fileInfoDic.Clear();

            // 获取所有子目录信息
            var directoryInfos = directoryInfo.GetDirectories();
            foreach (var info in directoryInfos)
            {
                if (filterDirectorys.Contains(info.Name))
                {
                    continue;
                }
                
                // 获取目录下所有非过滤后缀的文件
                var fileInfos = FileUtility.GetTotalFiles(info, new List<FileInfo>(), _filterSuffixes);
                _fileInfoDic.Add(info.Name, fileInfos);
            }

            var total = 0;
            foreach (var fileInfos in _fileInfoDic.Values)
            {
                total += fileInfos.Count;
            }

            var index = 0;
            foreach (var abName in _fileInfoDic.Keys)
            {
                var fileInfos = _fileInfoDic[abName];
                foreach (var fileInfo in fileInfos)
                {
                    // 获取相对路径
                    var dataPath = fileInfo.FullName[fileInfo.FullName.IndexOf("Assets", StringComparison.Ordinal)..];
                    
                    // 显示进度
                    var isCancel = EditorUtility.DisplayCancelableProgressBar("Collect Assets",
                        $"Collecting Path：{dataPath}", (float)index++ / (total - 1));
                    if (isCancel)
                    {
                        AppendToLog($"Collect operation be cancel");
                        return;
                    }
                    
                    // 创建单个资源信息
                    var assetInfo = new AssetBundlesCollections.AssetInfo(dataPath, fileInfo.Length, fileInfo.Name, GenerateFileSHA256Hash(fileInfo.FullName));
                    // 将资源信息添加到配置文件
                    _assetsColletion_Temp.Add(abName.ToLower(), assetInfo);
                }
            }
            
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 将选中的AB包文件移动到StreamingAssets目录
        /// 并更新该目录下的AB包清单文件
        /// </summary>
        private void MoveABToStreamingAssets()
        {
            // 确保目标路径存在
            if (!Directory.Exists(AB_COPY_PATH))
            {
                Directory.CreateDirectory(AB_COPY_PATH);
                AssetDatabase.Refresh();
                AppendToLog($" Create Path Of StreamingAssets ：{AB_COPY_PATH}");
            }
            
            // 先删除原有内容
            var files = Directory.GetFiles(AB_COPY_PATH);
            foreach (var file in files)
            {
                File.Delete(file);
            }
            AssetDatabase.Refresh();
            
            // 获取选中的资源
            var selAssets = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            if (selAssets.Length == 0)
            {
                return;
            }

            var totalCount = selAssets.Length / 2;
            // 拷贝选中的AB包文件到目标路径
            for (var i = 0; i < selAssets.Length; i++)
            {
                var isCancel = EditorUtility.DisplayCancelableProgressBar("Copying To StreamingAssets",
                    $"Processing：{selAssets[i].name}", i / (float)totalCount);

                if (isCancel)
                {
                    return;
                }
                
                var assetPath = AssetDatabase.GetAssetPath(selAssets[i]);
                var fileName = assetPath[(assetPath.LastIndexOf('/') + 1)..];
                // 仅处理AB包文件
                if (fileName.IndexOf(FileUtility.AbSuffix, StringComparison.Ordinal) == -1)
                {
                    continue;
                }

                AssetDatabase.CopyAsset(assetPath, $"{AB_COPY_PATH}/{fileName}");
            }
            
            EditorUtility.ClearProgressBar();
            
            // 更新StreamingAssets目录下的AB包清单文件
            CreateAssetBundleListFile(AB_COPY_PATH, $"{AB_COPY_PATH}{FileUtility.ListFileDefaultName}");
        }
        
        /// <summary>
        /// 创建AssetBundle清单文件（JSON格式）
        /// 包含每个AB包的名称、大小、hash值、依赖项等信息
        /// </summary>
        /// <param name="outPath">清单文件输出根路径</param>
        /// <param name="filePath">清单文件完整路径</param>
        private void CreateAssetBundleListFile(string outPath, string filePath)
        {
            // 根据ServerData路径下的内容来生成
            if (!Directory.Exists(outPath))
            {
                Directory.CreateDirectory(outPath);
            }

            AppendToLog($"---Start Create List File---");
            UnityEngine.AssetBundle mainBundle = null;
            try
            {
                // 加载包含 manifest 的 AssetBundle
                mainBundle = UnityEngine.AssetBundle.LoadFromFile(_mainBundlePath);
                if (!mainBundle)
                {
                    AppendToLog($"Failed to load main AssetBundle from: {_mainBundlePath}");
                    return;
                }

                // 从加载的 AssetBundle 中获取 AssetBundleManifest 对象
                var manifest = mainBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                if (!manifest)
                {
                    AppendToLog("Failed to load AssetBundleManifest object from the bundle.");
                    return;
                }

                // 获取AB包目录下所有文件
                var directoryInfo = Directory.CreateDirectory(outPath);
                var fileInfos = new List<FileInfo>(directoryInfo.GetFiles());

                // 构建清单数据
                var collection = new ABPackageCollection();
                foreach (var abInfo in fileInfos)
                {
                    // 仅处理AB包文件
                    if (abInfo.Extension == FileUtility.AbSuffix)
                    {
                        // 获取依赖包
                        var dependencies = manifest.GetAllDependencies(abInfo.Name.Substring(0, abInfo.Name.LastIndexOf('.')));
                        // 创建AB包信息对象（名称、大小、Hash）
                        var packageInfo = new ABPackageInfo(abInfo.Name, abInfo.Length, GenerateFileSHA256Hash(abInfo.FullName), dependencies);
                        collection.TryAdd(abInfo.Name, packageInfo);
                    }
                }
                
                // 保存为JSON文件
                JsonManager.Instance.SaveToJson(collection, filePath);
                AssetDatabase.Refresh();
                AppendToLog($"AssetBundle List File Created : {filePath}");
            }
            catch (Exception e)
            {
                AppendToLog($"Create error: {e.Message}\n{e.StackTrace}");
                AppendToLog($"---End Create List File---");
            }
            finally
            {
                // 卸载加载的 AssetBundle 以释放内存
                if (mainBundle)
                {
                    mainBundle.Unload(false); // false 表示不销毁加载的资源实例，仅卸载包本身
                }
                UnityEngine.AssetBundle.UnloadAllAssetBundles(true);
            }
        }
        
        /// <summary>
        /// 清空所有AB包标签
        /// </summary>
        private void ClearAssetLabel()
        {
            // 检查资源输入路径是否存在，不存在则创建并提示
            if (!Directory.Exists(AssetsInputPath))
            {
                Debug.Log($"资源输入路径不存在：{AssetsInputPath}，已自动创建，请放入待打包资源后重试");
                Directory.CreateDirectory(AssetsInputPath);
                return;
            }
            
            // 初始化目录信息
            var directoryInfo = Directory.CreateDirectory(AssetsInputPath);
            AssetBundlesCollections.Instance.Clear();
            _fileInfoDic.Clear();

            // 获取所有子目录信息
            var directoryInfos = directoryInfo.GetDirectories();
            foreach (var info in directoryInfos)
            {
                // 获取目录下所有非过滤后缀的文件
                var fileInfos = FileUtility.GetTotalFiles(info, new List<FileInfo>(), _filterSuffixes);
                _fileInfoDic.Add(info.Name, fileInfos);
            }
            
            // 为每个文件设置AB包标签，并记录到配置文件
            foreach (var abName in _fileInfoDic.Keys)
            {
                var fileInfos = _fileInfoDic[abName];
                foreach (var fileInfo in fileInfos)
                {
                    // 转换为Unity资源路径（相对路径）
                    var dataPath = fileInfo.FullName[fileInfo.FullName.IndexOf("Assets", StringComparison.Ordinal)..];
                    var importer = AssetImporter.GetAtPath(dataPath);
                    if (!importer)
                    {
                        continue;
                    }

                    if (importer.assetBundleName != "")
                    {
                        importer.assetBundleName = "";
                    }
                }
            }
        }
        
        /// <summary>
        /// 为资源自动设置AssetBundle标签
        /// 按目录结构自动分配AB包名称，并生成对应的枚举脚本
        /// </summary>
        private void EditAssetLabel()
        {
            if (abNameToDifferenceInfos.Count == 0)
            {
                AppendToLog($"No Differences");
                return;
            }
            
            // abNameToDifferenceInfos和_assetsCollection_Release.assetBundleInfos相同情况，第一次全量打包或全量更新（所有包都变化）
            if (!_assetsCollection_Release || abNameToDifferenceInfos.Count == _assetsCollection_Release.assetBundleInfos.Count)
            {
                var index = 0;
                var total = 0;
                foreach (var list in abNameToDifferenceInfos.Values)
                {
                    total += list.Count;
                }
            
                // 为每个文件设置AB包标签，并记录到配置文件
                foreach (var abName in abNameToDifferenceInfos.Keys)
                {
                    var assetInfos = abNameToDifferenceInfos[abName];
                    foreach (var assetInfo in assetInfos)
                    {
                        var isCancel = EditorUtility.DisplayCancelableProgressBar(
                            "Setting AssetBundle Lable", 
                            $"handing File: {assetInfo.name}",
                            (float)index / (total - 1));

                        if (isCancel)
                        {
                            Debug.Log($"已取消设置AB包标签操作");
                            return;
                        }
                    
                        var importer = AssetImporter.GetAtPath(assetInfo.assetPath);
                        if (!importer)
                        {
                            AppendToLog($"Setting Label error: {assetInfo.assetPath}");
                            continue;
                        }
                    
                        // 设置AB包名称（小写）
                        importer.assetBundleName = abName.ToLower();
                        ++index;
                    }
                }
            
                AppendToLog($"Setting Lables：[{string.Join(',', abNameToDifferenceInfos.Keys)}]");
                
                EditorUtility.ClearProgressBar();
            }
            else if (abNameToDifferenceInfos.Count > 0)
            {
                // 计算需要设置标签的资源
                var tempDic = new Dictionary<string, List<AssetBundlesCollections.AssetInfo>>();
                foreach (var assetBundleInfo in _assetsCollection_Release.assetBundleInfos)
                {
                    // 若差异字典中存在该AB包名称，说明这个包需要重新打包
                    if (abNameToDifferenceInfos.ContainsKey(assetBundleInfo.assetBundleName))
                    {
                        tempDic.Add(assetBundleInfo.assetBundleName, assetBundleInfo.assetInfos);
                    }
                }
                var index = 0;
                var total = 0;
                foreach (var list in tempDic.Values)
                {
                    total += list.Count;
                }
                
                // 遍历发布配置
                foreach (var assetBundleInfo in _assetsCollection_Release.assetBundleInfos)
                {
                    // 若差异字典中存在该AB包名称，说明这个包需要重新打包
                    if (abNameToDifferenceInfos.ContainsKey(assetBundleInfo.assetBundleName))
                    {
                        var assetInfos = assetBundleInfo.assetInfos;
                        foreach (var assetInfo in assetInfos)
                        {
                            var isCancel = EditorUtility.DisplayCancelableProgressBar(
                                "Setting AssetBundle Lable", 
                                $"handing File: {assetInfo.name}",
                                (float)index / (total - 1));

                            if (isCancel)
                            {
                                Debug.Log($"已取消设置AB包标签操作");
                                return;
                            }
                    
                            var importer = AssetImporter.GetAtPath(assetInfo.assetPath);
                            if (!importer)
                            {
                                AppendToLog($"Setting Label error: {assetInfo.assetPath}");
                                continue;
                            }
                    
                            // 设置AB包名称（小写）
                            importer.assetBundleName = assetBundleInfo.assetBundleName.ToLower();
                            ++index;
                        }
                    }
                    // 差异字典中不存在AB包，说明这个包没有变化，不用重新打包，那就将标签设置为None
                    else
                    {
                        var assetInfos = assetBundleInfo.assetInfos;
                        foreach (var assetInfo in assetInfos)
                        {
                            var importer = AssetImporter.GetAtPath(assetInfo.assetPath);
                            if (importer.assetBundleName != "")
                            {
                                importer.assetBundleName = "";
                            }
                        }
                    }
                }
                
                AppendToLog($"Setting Lables：[{string.Join(',', abNameToDifferenceInfos.Keys)}]");
                
                EditorUtility.ClearProgressBar();
            }
        }
        
        /// <summary>
        /// 上传当前平台的AB包文件到配置的服务器
        /// 支持分块上传、身份验证、自动/自定义上传字节数配置
        /// </summary>
        private void UpLoadAssetBundlesData()
        {
            // 获取AB包目录
            var directory = Directory.CreateDirectory(_outputPath);
            var fileInfos = directory.GetFiles();

            // 筛选需要上传的文件（AB包和清单文件）
            var uploadList = new List<FileInfo>();
            foreach (var fileInfo in fileInfos)
            {
                if (fileInfo.Extension != FileUtility.AbSuffix && fileInfo.Extension != ".json")
                    continue;

                uploadList.Add(fileInfo);
            }

            // 初始化上传计数
            upLoadmaxNum = uploadList.Count;
            nowUpLoadFinishedNum = 0;

            // 遍历上传文件
            foreach (var fileInfo in uploadList)
            {
                UpLoadHttp(fileInfo.FullName, fileInfo.Name);
            }
        }

        /// <summary>
        /// 异步上传单个文件到服务器（HTTP POST）
        /// </summary>
        /// <param name="filePath">本地文件完整路径</param>
        /// <param name="fileName">文件名（用于服务器接收）</param>
        private async void UpLoadHttp(string filePath, string fileName)
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        // 创建HTTP请求
                        var req = WebRequest.Create(new Uri(serverIP)) as HttpWebRequest;
                        if (req == null)
                        {
                            return;
                        }
                        
                        req.Method = WebRequestMethods.Http.Post;
                        req.ContentType = "multipart/form-data;boundary=MrQiu";
                        req.Timeout = 500000;

                        // 配置身份验证（如果启用）
                        if (uploadUseUser)
                            req.Credentials = new NetworkCredential(userName, password);
                        req.PreAuthenticate = true;

                        // 构建请求头
                        var head = "--MrQiu\r\n" +
                                   $"Content-Disposition:form-data;name=\"file\";filename=\"{fileName}\"\r\n" +
                                   "Content-Type:application/octet-stream\r\n\r\n";
                        var headBytes = Encoding.UTF8.GetBytes(head);
                        var endBytes = Encoding.UTF8.GetBytes("\r\n--MrQiu--\r\n");

                        // 读取文件并上传
                        using (var fileStream = File.OpenRead(filePath))
                        {
                            req.ContentLength = headBytes.Length + fileStream.Length + endBytes.Length;

                            // 自动计算分块大小
                            long chunkSize;
                            if (uploadBytesIsAutoSetting)
                            {
                                chunkSize = req.ContentLength switch
                                {
                                    // >=100MB
                                    >= 1024 * 1024 * 100 => 1024 * 1024,
                                    // 50~100MB
                                    >= 1024 * 1024 * 50 and < 1024 * 1024 * 100 => 65536,
                                    // 1~50MB
                                    > 1024 * 1024 and < 1024 * 1024 * 50 => 4096,
                                    _ => req.ContentLength
                                };
                            }
                            else
                            {
                                // 自定义分块大小
                                chunkSize = maxBytesCapacity;
                            }

                            // 写入请求数据
                            using var upLoadStream = req.GetRequestStream();
                            upLoadStream.Write(headBytes, 0, headBytes.Length);

                            var bytes = new byte[chunkSize];
                            var readLength = fileStream.Read(bytes, 0, bytes.Length);
                            while (readLength != 0)
                            {
                                upLoadStream.Write(bytes, 0, readLength);
                                readLength = fileStream.Read(bytes, 0, bytes.Length);
                            }

                            upLoadStream.Write(endBytes, 0, endBytes.Length);
                            upLoadStream.Close();
                            fileStream.Close();
                        }

                        // 获取响应并处理
                        var res = req.GetResponse() as HttpWebResponse;
                        if (res != null && res.StatusCode == HttpStatusCode.OK)
                        {
                            using (var stream = res.GetResponseStream())
                            using (var sr = new StreamReader(stream, Encoding.UTF8))
                            {
                                var responseText = sr.ReadToEnd();
                                if (!string.IsNullOrEmpty(responseText))
                                {
                                    //Debug.Log($"服务器响应：{responseText}");
                                }
                            }

                            Debug.Log($"{fileName} 上传成功，进度：{++nowUpLoadFinishedNum}/{upLoadmaxNum}");
                        }
                        else
                        {
                            Debug.LogError($"{fileName} 上传失败，状态码：{res?.StatusCode}");
                        }

                        res?.Close();
                    }
                    catch (Exception e)
                    {
                        ++nowUpLoadFinishedNum;
                        Debug.LogError($"上传文件 {fileName} 时发生错误：{e.Message}");
                    }
                });
            }
            catch (Exception e)
            {
                Debug.LogError($"上传异常：{e.Message}");
            }
        }

        /// <summary>
        /// 创建AssetBundle配置文件（AssetBundlesCollections.asset）
        /// 用于存储AB包与资源的映射关系
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="fileName"></param>
        private AssetBundlesCollections CreateCollection(string savePath, string fileName)
        {
            // 检查配置文件存储路径是否存在，不存在则创建
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            // 创建ScriptableObject实例并保存为资源文件
            var collections = CreateInstance<AssetBundlesCollections>();
            AssetDatabase.CreateAsset(collections, $"{savePath}{fileName}");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            AppendToLog($"Config ScriptableObject be created at：\n{savePath}{fileName}");

            return collections;
        }
        
        /// <summary>
        /// 拷贝热更新程序集
        /// </summary>
        private static void MoveHotUpdateAssembly()
        {
            const string sourcesPath = @"D:\UnityProject\TurnDemo\HybridCLRData\HotUpdateDlls\StandaloneWindows\Assembly-CSharp-Game-HotUpdate.dll";
            File.Copy(sourcesPath,  targetPath, true);
            AssetDatabase.Refresh();
        }
        
        private void BuildAssetBundles()
        {
            // 检查输出路径是否存在
            if (!Directory.Exists(_outputPath))
            {
                AppendToLog($"Output path does not exist：{_outputPath}，Please create path");
                return;
            }

            // 检查资源输入路径是否有文件
            if (Directory.CreateDirectory(AssetsInputPath).GetFiles().Length == 0)
            {
                AppendToLog("no files to be packaged at the resource input path");
                return;
            }

            AppendToLog("--- Starting Build ---");
            // 清空输出目录原有文件
            var info = Directory.CreateDirectory(_outputPath);
            var infos = info.GetFiles();
            foreach (var fileInfo in infos)
            {
                File.Delete(fileInfo.FullName);
            }

            // 执行AB包打包
            var startTime = DateTime.Now;
            var manifest = BuildPipeline.BuildAssetBundles(_outputPath, _buildOptions, _targetPlatform);
            var duration = DateTime.Now - startTime;
            
            if (manifest)
            {
                AppendToLog($"Build successful! Took {duration.TotalSeconds:F2} seconds.");
                AppendToLog($"Build Count：{manifest.GetAllAssetBundles().Length}");
                AppendToLog($"Build Include：\n{string.Join(',', manifest.GetAllAssetBundles())}.");
            }
            else
            {
                AppendToLog("Build failed! Check console for errors.");
            }
            AssetDatabase.Refresh();

            // 重命名AB包文件（统一后缀为.assetbundle）
            var directoryInfo = Directory.CreateDirectory(_outputPath);
            var fileInfos = directoryInfo.GetFiles();
            foreach (var fileInfo in fileInfos)
            {
                // 忽略manifest和meta文件
                if (fileInfo.Extension is ".manifest" or ".meta")
                {
                    if (fileInfo.Extension == ".meta")
                    {
                        File.Delete(fileInfo.FullName);
                    }
                    continue;
                }

                // 重命名为.assetbundle后缀
                var newFileName = Path.ChangeExtension(fileInfo.FullName, FileUtility.AbSuffix);
                if (File.Exists(newFileName))
                {
                    File.Delete(newFileName);
                }
                File.Move(fileInfo.FullName, newFileName);
            }
            AppendToLog($"Rename Extension To：{FileUtility.AbSuffix}");
            
            AssetDatabase.Refresh();
            AppendToLog("--- Build End ---");
        }

        private void CopyToServerData()
        {
            // 检查输出路径是否存在
            if (!Directory.Exists(_outputPath))
            {
                AppendToLog($"Output path does not exist：{_outputPath}，Please create path");
                return;
            }
            
            var info = Directory.CreateDirectory(_outputPath);
            var infos = info.GetFiles();
            foreach (var fileInfo in infos)
            {
                if (fileInfo.Extension == ".meta")
                {
                    continue;
                }
                
                File.Copy(fileInfo.FullName, $"{serverDataPath}{fileInfo.Name}", true);
            }
            
            AssetDatabase.Refresh();
        }

        private void CleanOutputDirectory()
        {
            if (Directory.Exists(_outputPath))
            {
                try
                {
                    Directory.Delete(_outputPath, true); // 递归删除
                    Directory.CreateDirectory(_outputPath); // 重建空目录
                    AppendToLog($"Cleaned output directory: {_outputPath}");
                }
                catch (Exception e)
                {
                    AppendToLog($"Failed to clean directory: {e.Message}");
                }
                AssetDatabase.Refresh();
            }
            else
            {
                AppendToLog($"Output directory does not exist: {_outputPath}");
            }
        }

        private void AnalyzeDependencies()
        {
            if (string.IsNullOrEmpty(_mainBundlePath) || !File.Exists(_mainBundlePath))
            {
                AppendToLog("Invalid Main AssetBundle file path.");
                return;
            }
            
            AppendToLog("--- Analyzing Dependencies ---");
            AppendToLog($"Loading AssetBundle from: {_mainBundlePath}");

            UnityEngine.AssetBundle mainBundle = null;
            try
            {
                // 加载包含 manifest 的 AssetBundle
                mainBundle = UnityEngine.AssetBundle.LoadFromFile(_mainBundlePath);
                if (!mainBundle)
                {
                    AppendToLog($"Failed to load main AssetBundle from: {_mainBundlePath}");
                    return;
                }

                // 从加载的 AssetBundle 中获取 AssetBundleManifest 对象
                var manifest = mainBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                if (!manifest)
                {
                    AppendToLog("Failed to load AssetBundleManifest object from the bundle.");
                    return;
                }

                // 使用 manifest 分析依赖
                var allAssetBundleNames = manifest.GetAllAssetBundles();
                AppendToLog($"Found {allAssetBundleNames.Length} bundles in manifest：");
                foreach (var abName in allAssetBundleNames)
                {
                    var dependencies = manifest.GetAllDependencies(abName);
                    AppendToLog($"Bundle '{abName}' depends on: [{string.Join(", ", dependencies)}]");
                }
            }
            catch (Exception e)
            {
                AppendToLog($"Analysis error: {e.Message}\n{e.StackTrace}");
            }
            finally
            {
                // 卸载加载的 AssetBundle 以释放内存
                if (mainBundle)
                {
                    mainBundle.Unload(false); // false 表示不销毁加载的资源实例，仅卸载包本身
                }
            }

            AppendToLog("--- Analysis End ---");
        }

        private void OpenOutputDirectory()
        {
            if (Directory.Exists(_outputPath))
            {
                EditorUtility.RevealInFinder(_outputPath);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", $"Directory does not exist:\n{_outputPath}", "OK");
            }
        }

        private void AppendToLog(string message)
        {
            _buildLog += $"{message}\n";
            // 自动滚动到底部
            _scrollPos.y = float.MaxValue;
        }
        
        /// <summary>
        /// 计算文件内容的 SHA256 哈希值
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>SHA256 哈希值的十六进制字符串</returns>
        public static string GenerateFileSHA256Hash(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var fileStream = File.OpenRead(filePath);
            var hashBytes = sha256.ComputeHash(fileStream);

            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}