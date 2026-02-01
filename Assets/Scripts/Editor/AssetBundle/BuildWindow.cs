using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.AssetBundles.Update.Collection;
using Core.DataPersistence.Json;
using Core.Utility;
using CustomEditor.ScriptGeneration;
using Editor.Menu;
using Framework.Editor.Generation;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor.AssetBundle
{
    /// <summary>
    /// AssetBundle打包窗口编辑器类
    /// 提供可视化界面用于配置、构建、上传AssetBundle资源包
    /// </summary>
    public class BuildWindow : EditorWindow
    {
        /// <summary>
        /// 目标平台选择索引（对应targetPlatformStrs数组）
        /// </summary>
        private int platformIndex;

        /// <summary>
        /// 压缩模式选择索引（对应compressModeStrs数组）
        /// </summary>
        private int compressIndex;

        /// <summary>
        /// 支持的打包目标平台名称数组
        /// </summary>
        private readonly string[] targetPlatformStrs = { "PC", "IOS", "Android" };

        /// <summary>
        /// AssetBundle压缩模式名称数组
        /// </summary>
        private readonly string[] compressModeStrs = { "Uncompress", "LZ4"};

        /// <summary>
        /// 资源上传服务器地址
        /// </summary>
        private string serverIP = "http://...";

        /// <summary>
        /// AssetBundle拷贝到StreamingAssets的目标路径
        /// </summary>
        private const string AB_COPY_PATH = "Assets/StreamingAssets/AssetBundles/";

        /// <summary>
        /// 上传服务器的用户名
        /// </summary>
        private string userName = "userName";

        /// <summary>
        /// 上传服务器的密码
        /// </summary>
        private string password = "password";

        /// <summary>
        /// 单次上传的最大字节数（自定义模式下生效）
        /// </summary>
        private uint maxBytesCapacity = 4096;

        /// <summary>
        /// 待打包资源的输入根路径
        /// </summary>
        private const string AssetsInputPath = "Assets/Editor/ArtRes/";

        /// <summary>
        /// AssetBundle打包输出根路径
        /// </summary>
        private const string AssetBundlesOutPath = "Assets/AssetBundleDatas/";

        /// <summary>
        /// 待上传文件总数
        /// </summary>
        private static int upLoadmaxNum;

        /// <summary>
        /// 已完成上传的文件数
        /// </summary>
        private static int nowUpLoadFinishedNum;

        /// <summary>
        /// 滚动视图位置
        /// </summary>
        private Vector2 pos;

        /// <summary>
        /// AssetBundle打包选项配置
        /// </summary>
        private BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.None;

        // AssetBundle打包高级配置项
        /// <summary>
        /// 是否为AssetBundle名称追加哈希值
        /// </summary>
        private bool isAppendHashToAssetBundleName;
        /// <summary>
        /// 是否使用内容哈希（仅Unity2021.3以下版本生效）
        /// </summary>
        private bool isUseContentHash;
        /// <summary>
        /// 是否忽略类型树变更
        /// </summary>
        private bool isIgnoreTypeTreeChanges;
        /// <summary>
        /// 是否仅执行打包模拟（不实际生成文件）
        /// </summary>
        private bool isDryRunBuild;
        /// <summary>
        /// 是否启用AssetBundle保护
        /// </summary>
        private bool isEnableProtection;
        /// <summary>
        /// 是否启用严格模式（严格检查打包错误）
        /// </summary>
        private bool isStrictMode;

        /// <summary>
        /// 打包配置折叠面板是否展开
        /// </summary>
        private bool isFoldExpand = true;

        /// <summary>
        /// 上传时是否启用用户身份验证
        /// </summary>
        private bool uploadUseUser;

        /// <summary>
        /// 是否显示明文密码
        /// </summary>
        private bool showPassWord;

        // 上传字节数配置模式
        /// <summary>
        /// 上传字节数是否自动配置
        /// </summary>
        private bool uploadBytesIsAutoSetting = true;
        /// <summary>
        /// 上传字节数是否自定义配置（与auto互斥）
        /// </summary>
        private bool uploadBytesIsCustomSetting;

        // 存储待处理文件信息的字典：Key为目录名，Value为该目录下的文件列表
        private readonly Dictionary<string, List<FileInfo>> _fileInfoDic = new Dictionary<string, List<FileInfo>>();

        // 需要过滤的文件后缀（打包时忽略）
        private readonly string[] _filterSuffixes = { ".meta" };

        // AssetBundle类型枚举默认名称（用于生成枚举脚本）
        private readonly string[] defaultNames = {
            "UI", "Scene", "Music", "Camera", "Video", "GameConfig", "HotUpdate",
            "SpriteAtlas", "Texture"
        };

        // AssetBundle类型枚举脚本生成路径
        private readonly string _filePath = $"{Application.dataPath}/Scripts/Core/AssetBundles/Management/EAssetBundleType.cs";

        /// <summary>
        /// 编辑器菜单入口：打开AssetBundle打包窗口
        /// </summary>
        [MenuItem("GameTool/AssetBundle/Build Package Window")]
        private static void OpenWindow()
        {
            var window = GetWindow<BuildWindow>("Build Window");
            window.Show();
        }

        /// <summary>
        /// 绘制编辑器窗口GUI
        /// </summary>
        private void OnGUI()
        {
            pos = GUILayout.BeginScrollView(pos);

            // 资源输入路径显示（不可编辑）
            GUILayout.Label(new GUIContent("AssetsInputPath", "待打包资源的根路径，所有AB包资源需放在此路径下"));
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(AssetsInputPath);
            EditorGUI.EndDisabledGroup();

            // 创建资源输入路径按钮
            if (GUILayout.Button(new GUIContent("Create Path", "创建当前配置的资源输入路径")))
            {
                if (!Directory.Exists(AssetsInputPath))
                {
                    Directory.CreateDirectory(AssetsInputPath);
                    AssetDatabase.Refresh();
                    Debug.Log($"已创建资源输入路径：{AssetsInputPath}");
                }
            }

            // 目标平台选择
            GUILayout.Label(new GUIContent("TargetPlatform", "选择AssetBundle打包的目标平台"));
            platformIndex = GUILayout.Toolbar(platformIndex, targetPlatformStrs);

            // 打包输出路径显示（不可编辑）
            GUILayout.Label(new GUIContent("AssetBundlesOutPath", "AssetBundle打包输出路径（按平台区分）"));
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(AssetBundlesOutPath + targetPlatformStrs[platformIndex]);
            EditorGUI.EndDisabledGroup();

            // 创建打包输出路径按钮
            if (GUILayout.Button(new GUIContent("Create Path", "创建当前平台的AB包输出路径")))
            {
                var targetPath = AssetBundlesOutPath + targetPlatformStrs[platformIndex];
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                    AssetDatabase.Refresh();
                    Debug.Log($"已创建AB包输出路径：{targetPath}");
                }
            }

            // 服务器地址配置
            GUILayout.Label(new GUIContent("ServerAddress", "AB包上传的服务器地址"));
            serverIP = GUILayout.TextField(serverIP);

            // AB包配置文件路径显示（不可编辑）
            GUILayout.Label(new GUIContent("AssetBundleConfigPath", "AssetBundle配置文件存储路径"));
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(MenuItemTools.AssetPath + "AssetBundlesCollections.asset");
            EditorGUI.EndDisabledGroup();
            
            // 创建AB包配置文件按钮
            if (GUILayout.Button(new GUIContent("Create Config", "创建AssetBundle配置文件（AssetBundlesCollections.asset）")))
            {
                CreateCollection();
            }
            
            // 拷贝热更新程序集
            if (GUILayout.Button(new GUIContent("Copy HotUpdate Assembly", "转存热更新程序集")))
            {
                MoveHotUpdateAssembly();
            }
            
            // 设置AB包标签按钮
            if (GUILayout.Button(new GUIContent("Set AssetBundleLabel", "为资源自动设置AssetBundle标签")))
            {
                EditAssetLabel();
            }
            
            // 清空AB包标签按钮
            if (GUILayout.Button(new GUIContent("Clear All AssetBundleLabel", "清空所有资源设置的AssetBundle标签")))
            {
                ClearAssetLabel();
            }

            // AB包路径重复显示（与输出路径一致，用于确认）
            GUILayout.Label(new GUIContent("AssetBundlePath", "当前平台AB包最终输出路径"));
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(AssetBundlesOutPath + targetPlatformStrs[platformIndex]);
            EditorGUI.EndDisabledGroup();

            isFoldExpand = EditorGUILayout.Foldout(isFoldExpand, new GUIContent("BuildConfig", "AssetBundle打包高级配置"), true);
            if(isFoldExpand)
            {
                // 压缩模式选择
                GUILayout.Label(new GUIContent("Compressed Mode", "选择AB包的压缩模式"));
                compressIndex = GUILayout.Toolbar(compressIndex, new[] {
                    new GUIContent(compressModeStrs[0], "不压缩，打包/加载速度快，体积大"),
                    new GUIContent(compressModeStrs[1], "LZ4块压缩，平衡体积与加载速度")
                });

                // 高级打包选项开关
                isAppendHashToAssetBundleName = GUILayout.Toggle(isAppendHashToAssetBundleName, new GUIContent("AppendHashToAssetBundleName", "为AB包名称追加哈希值（用于版本区分）"));
                isUseContentHash = GUILayout.Toggle(isUseContentHash, new GUIContent("UseContentHash", "使用内容哈希（仅Unity2021.3以下版本生效）"));
                isIgnoreTypeTreeChanges = GUILayout.Toggle(isIgnoreTypeTreeChanges, new GUIContent("IgnoreTypeTreeChanges", "忽略类型树变更（减少包体积）"));
                isDryRunBuild = GUILayout.Toggle(isDryRunBuild, new GUIContent("DryRunBuild", "模拟打包（仅检查错误，不生成文件）"));
                isEnableProtection = GUILayout.Toggle(isEnableProtection, new GUIContent("EnableProtection", "启用AB包保护（防止篡改）"));
                isStrictMode = GUILayout.Toggle(isStrictMode, new GUIContent("Strict Mode", "严格模式（严格检查打包错误）"));
            }

            // 执行打包按钮
            if (GUILayout.Button(new GUIContent("Build AssetBundles", "开始构建当前平台的AssetBundle包")))
            {
                BuildAssetBundles();
            }

            // AB包清单文件路径显示（不可编辑）
            GUILayout.Label(new GUIContent("ABListFilePath", "AB包清单文件（包含MD5、大小等信息）输出路径"));
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(AssetBundlesOutPath + targetPlatformStrs[platformIndex] + "/" + "AssetBundleListInfo.json");
            EditorGUI.EndDisabledGroup();

            // 生成AB包清单文件按钮
            if (GUILayout.Button(new GUIContent("Create ABListFile", "生成AB包清单文件（JSON格式）")))
            {
                CreateAssetBundleListFile(
                    AssetBundlesOutPath, 
                    AssetBundlesOutPath + targetPlatformStrs[platformIndex], 
                    $"{AssetBundlesOutPath}{targetPlatformStrs[platformIndex]}/{FileUtility.ListFileDefaultName}"
                );
            }

            // AB包拷贝路径显示（不可编辑）
            GUILayout.Label(new GUIContent("AssetBundleTransferPath", "AB包拷贝到StreamingAssets的目标路径"));
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(AB_COPY_PATH);
            EditorGUI.EndDisabledGroup();

            // 拷贝AB包到StreamingAssets按钮
            if (GUILayout.Button(new GUIContent("Move AssetBundle To StreamingAssets", "将选中的AB包拷贝到StreamingAssets目录")))
            {
                MoveABToStreamingAssets();
            }

            // 预览服务器地址显示（与上传地址一致）
            GUILayout.Label(new GUIContent("PreviewServerIP", "预览用服务器地址（与上传地址一致）"));
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(serverIP);
            EditorGUI.EndDisabledGroup();

            // 上传身份验证开关
            uploadUseUser = GUILayout.Toggle(uploadUseUser, new GUIContent("Verify Identity", "上传时启用用户名/密码身份验证"));

            // 身份验证配置（仅当开关开启时显示）
            if (uploadUseUser)
            {
                GUILayout.Label(new GUIContent("UserName", "服务器验证用户名"));
                userName = GUILayout.TextField(userName);

                showPassWord = GUILayout.Toggle(showPassWord, new GUIContent("Show Password", "显示/隐藏密码"));

                GUILayout.Label(new GUIContent("Password", "服务器验证密码"));
                password = showPassWord ? GUILayout.TextField(password) : GUILayout.PasswordField(password, '*');
            }

            // 上传字节数配置
            GUILayout.Label(new GUIContent("Max UpLoad-Bytes Per One Time", "单次上传的最大字节数配置"));
            uploadBytesIsCustomSetting = GUILayout.Toggle(!uploadBytesIsAutoSetting, new GUIContent("Custom", "自定义配置"));
            uploadBytesIsAutoSetting = GUILayout.Toggle(!uploadBytesIsCustomSetting, new GUIContent("Auto", "自动配置（按文件大小分级）"));

            // 自定义上传字节数输入（仅当自定义模式开启时显示）
            if (!uploadBytesIsAutoSetting)
            {
                GUILayout.Label(new GUIContent("Custom Max UpLoad-Bytes Per One Time", "自定义单次上传最大字节数"));
                maxBytesCapacity = uint.Parse(GUILayout.TextField(maxBytesCapacity.ToString()), NumberStyles.Number);
            }

            // 上传AB包按钮
            if (GUILayout.Button(new GUIContent("Upload AssetBundleDatas", "上传当前平台的AB包到服务器")))
            {
                UpLoadAssetBundlesData();
            }

            GUILayout.EndScrollView();
        }

        /// <summary>
        /// 创建AssetBundle配置文件（AssetBundlesCollections.asset）
        /// 用于存储AB包与资源的映射关系
        /// </summary>
        private static void CreateCollection()
        {
            // 检查配置文件存储路径是否存在，不存在则创建
            if (!Directory.Exists(MenuItemTools.AssetPath))
            {
                Directory.CreateDirectory(MenuItemTools.AssetPath);
                Debug.Log($"已创建配置文件存储路径：{MenuItemTools.AssetPath}，用于存放AB包配置文件");
            }

            // 创建ScriptableObject实例并保存为资源文件
            var collections = CreateInstance<AssetBundlesCollections>();
            AssetDatabase.CreateAsset(collections, $"{MenuItemTools.AssetPath}AssetBundlesCollections.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 拷贝热更新程序集
        /// </summary>
        private static void MoveHotUpdateAssembly()
        {
            const string sourcesPath = @"D:\UnityProject\TurnDemo\HybridCLRData\HotUpdateDlls\StandaloneWindows\Assembly-CSharp-Game-HotUpdate.dll";
            const string targetPath = @"D:\UnityProject\TurnDemo\Assets\Editor\ArtRes\HotUpdate\Assembly-CSharp-Game-HotUpdate.dll.bytes";
            File.Copy(sourcesPath,  targetPath, true);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 为资源自动设置AssetBundle标签
        /// 按目录结构自动分配AB包名称，并生成对应的枚举脚本
        /// </summary>
        private void EditAssetLabel()
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
                    
                    // 设置AB包名称（小写）
                    importer.assetBundleName = abName.ToLower();
                    // 将资源信息添加到配置文件
                    AssetBundlesCollections.Instance.Add(importer.assetBundleName, new AssetBundlesCollections.AssetInfo(dataPath, fileInfo.Length, fileInfo.Name));
                }
            }

            // 生成AssetBundle类型枚举脚本
            IScriptGenerator scriptGenerator = new EnumGenerator(_fileInfoDic.Keys, defaultNames, _filePath, "Core.AssetBundles.Management");
            scriptGenerator.GenerateScript();
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
        /// 构建AssetBundle包
        /// 根据当前配置的平台、压缩模式、高级选项生成AB包
        /// </summary>
        private void BuildAssetBundles()
        {
            // 检查输出路径是否存在
            var targetOutPath = $"{AssetBundlesOutPath}{targetPlatformStrs[platformIndex]}";
            if (!Directory.Exists(targetOutPath))
            {
                Debug.LogError($"AB包输出路径不存在：{targetOutPath}，请先点击Create Path创建路径");
                return;
            }

            // 检查资源输入路径是否有文件
            if (Directory.CreateDirectory(AssetsInputPath).GetFiles().Length == 0)
            {
                Debug.LogWarning("资源输入路径下无待打包文件，请先放入资源后再执行打包");
                return;
            }

            // 清空输出目录原有文件
            var info = Directory.CreateDirectory(targetOutPath);
            var infos = info.GetFiles();
            foreach (var fileInfo in infos)
            {
                File.Delete(fileInfo.FullName);
            }
            
            // 配置压缩模式
            assetBundleOptions = compressModeStrs[compressIndex] switch
            {
                "Uncompress" => BuildAssetBundleOptions.UncompressedAssetBundle,
                "LZ4" => BuildAssetBundleOptions.ChunkBasedCompression,
                _ => assetBundleOptions
            };

            // 配置高级打包选项
            if (isAppendHashToAssetBundleName)
                assetBundleOptions |= BuildAssetBundleOptions.AppendHashToAssetBundleName;
#if !UNITY_2021_3_OR_NEWER
            if (isUseContentHash)
                assetBundleOptions |= BuildAssetBundleOptions.UseContentHash;
#endif
            if (isIgnoreTypeTreeChanges)
                assetBundleOptions |= BuildAssetBundleOptions.IgnoreTypeTreeChanges;
            if (isDryRunBuild)
                assetBundleOptions |= BuildAssetBundleOptions.DryRunBuild;
            if (isEnableProtection)
                assetBundleOptions |= BuildAssetBundleOptions.EnableProtection;
            if (isStrictMode)
                assetBundleOptions |= BuildAssetBundleOptions.StrictMode;

            // 映射目标平台到Unity的BuildTarget枚举
            var buildTarget = targetPlatformStrs[platformIndex] switch
            {
                "PC" => BuildTarget.StandaloneWindows,
                "IOS" => BuildTarget.iOS,
                "Android" => BuildTarget.Android,
                _ => BuildTarget.NoTarget
            };

            // 执行AB包打包
            BuildPipeline.BuildAssetBundles(targetOutPath, assetBundleOptions, buildTarget);
            AssetDatabase.Refresh();

            // 重命名AB包文件（统一后缀为.assetbundle）
            var directoryInfo = Directory.CreateDirectory(targetOutPath);
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
                var newFileName = Path.ChangeExtension(fileInfo.FullName, ".assetbundle");
                if (File.Exists(newFileName))
                {
                    File.Delete(newFileName);
                }
                File.Move(fileInfo.FullName, newFileName);
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 创建AssetBundle清单文件（JSON格式）
        /// 包含每个AB包的名称、大小、MD5值等信息
        /// </summary>
        /// <param name="outPath">清单文件输出根路径</param>
        /// <param name="dirctoryPath">AB包所在目录</param>
        /// <param name="filePath">清单文件完整路径</param>
        private static void CreateAssetBundleListFile(string outPath, string dirctoryPath, string filePath)
        {
            // 确保输出路径存在
            if (!Directory.Exists(outPath))
            {
                Directory.CreateDirectory(outPath);
            }

            // 获取AB包目录下所有文件
            var directoryInfo = Directory.CreateDirectory(dirctoryPath);
            var fileInfos = directoryInfo.GetFiles();

            // 构建清单数据
            var collection = new ABPackageCollection();
            foreach (var info in fileInfos)
            {
                // 仅处理AB包文件
                if (info.Extension != AssetBundleManager.Instance.AbSuffix)
                {
                    continue;
                }

                // 创建AB包信息对象（名称、大小、MD5）
                var packageInfo = new ABPackageInfo(info.Name, info.Length, GetMD5(info.FullName));
                collection.TryAdd(info.Name, packageInfo);
            }

            // 保存为JSON文件
            JsonManager.Instance.SaveToJson(collection, filePath);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 将选中的AB包文件移动到StreamingAssets目录
        /// 并更新该目录下的AB包清单文件
        /// </summary>
        private static void MoveABToStreamingAssets()
        {
            // 确保目标路径存在
            if (!Directory.Exists(AB_COPY_PATH))
            {
                Directory.CreateDirectory(AB_COPY_PATH);
                AssetDatabase.Refresh();
                Debug.Log($"已创建StreamingAssets下的AB包路径：{AB_COPY_PATH}");
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
            
            // 拷贝选中的AB包文件到目标路径
            foreach (var obj in selAssets)
            {
                var assetPath = AssetDatabase.GetAssetPath(obj);
                var fileName = assetPath[(assetPath.LastIndexOf('/') + 1)..];
                // 仅处理AB包文件
                if (fileName.IndexOf(".assetbundle", StringComparison.Ordinal) == -1)
                {
                    continue;
                }

                AssetDatabase.CopyAsset(assetPath, $"{AB_COPY_PATH}/{fileName}");
            }

            // 更新StreamingAssets目录下的AB包清单文件
            CreateAssetBundleListFile(AB_COPY_PATH, AB_COPY_PATH, $"{AB_COPY_PATH}{FileUtility.ListFileDefaultName}");
        }

        /// <summary>
        /// 上传当前平台的AB包文件到配置的服务器
        /// 支持分块上传、身份验证、自动/自定义上传字节数配置
        /// </summary>
        private void UpLoadAssetBundlesData()
        {
            // 获取AB包目录
            var targetPath = AssetBundlesOutPath + targetPlatformStrs[platformIndex];
            var directory = Directory.CreateDirectory(targetPath);
            var fileInfos = directory.GetFiles();

            // 筛选需要上传的文件（AB包和清单文件）
            var uploadList = new List<FileInfo>();
            foreach (var fileInfo in fileInfos)
            {
                if (fileInfo.Extension != AssetBundleManager.Instance.AbSuffix && fileInfo.Extension != ".json")
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
                                    Debug.Log($"服务器响应：{responseText}");
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
        /// 计算文件的MD5哈希值
        /// 用于校验文件完整性
        /// </summary>
        /// <param name="filePath">文件完整路径</param>
        /// <returns>小写的32位MD5字符串</returns>
        private static string GetMD5(string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            MD5 md5 = new MD5CryptoServiceProvider();
            var md5Bytes = md5.ComputeHash(fileStream);
            fileStream.Close();

            var sb = new StringBuilder();
            foreach (var md5Byte in md5Bytes)
            {
                sb.Append(md5Byte.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}