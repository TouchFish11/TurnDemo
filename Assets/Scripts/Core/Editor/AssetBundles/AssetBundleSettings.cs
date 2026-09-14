using UnityEditor;
using UnityEngine;

namespace Core.Editor.AssetBundles
{
    /// <summary>
    /// AB 打包配置：跨项目可变数据集中在这里。复制工具到新项目后，只需改这个 SO 即可，无需再改代码。
    /// </summary>
    [CreateAssetMenu(fileName = "AssetBundleSettings", menuName = "GameTool/AssetBundle/Settings")]
    public class AssetBundleSettings : ScriptableObject
    {
        /// <summary>SO 固定存放路径（必须先于 SO 存在，故不能放进 SO 自身配置里）</summary>
        public const string SettingsPath = "Assets/Editor/AssetBundleSettings/AssetBundleSettings.asset";

        [Header("资源输入")]
        [Tooltip("待打包资源的根目录（相对 Assets 的路径），每个子文件夹会被打成一个 AB 包")]
        public string assetsInputPath = "Assets/Editor/ArtRes/";

        [Tooltip("收集时要跳过的子目录名（默认跳过 Texture 目录）")]
        public string[] filterDirectories = { "Texture" };

        [Tooltip("收集时要跳过的文件后缀（默认跳过 .meta 文件）")]
        public string[] filterSuffixes = { ".meta" };

        [Header("热更程序集")]
        [Tooltip("热更程序集 DLL 的拷贝目标目录（HybridCLR 生成的 DLL 拷到这里）")]
        public string hotUpdateAssemblyTargetPath = "Assets/Editor/ArtRes/HotUpdate/";

        [Tooltip("HybridCLR 生成的 DLL 源目录（相对项目根目录）")]
        public string hybridCLRAssemblySourcesPath = "HybridCLRData/HotUpdateDlls/StandaloneWindows64/";

        [Tooltip("参与热更的程序集名列表（用于拷贝 DLL 和生成依赖文件）")]
        public string[] hotUpdateAssemblies =
        {
            "HotUpdate.Common", "HotUpdate.Base", "HotUpdate.Game", "HotUpdate.UI", "HotUpdate.Update",
        };

        [Header("输出 / 保存")]
        [Tooltip("AssetBundle 集合快照（Temp / Release 两份 SO）的保存目录")]
        public string abSettingsSavePath = "Assets/Editor/AssetBundleSettings/";

        [Tooltip("首包 AB 拷贝到 StreamingAssets 的目录")]
        public string streamingAssetsCopyPath = "Assets/StreamingAssets/AssetBundles/";

        [Tooltip("服务器数据目录；留空则使用 Application.dataPath/ServerData")]
        public string serverDataPath = "";

        [Tooltip("AssetKeys / AssetBundleKeys 常量脚本的生成目录（相对 Application.dataPath，不含 Assets 前缀）")]
        public string generatedScriptDir = "Scripts/HotUpdate/Common/Generated";

        [Header("服务器")]
        [Tooltip("AB 上传服务器地址")]
        public string serverIP = "";

        [Tooltip("上传账号")]
        public string userName = "";

        [Tooltip("上传密码")]
        public string password = "";

        /// <summary>加载配置，不存在则在固定路径自动创建一份默认值</summary>
        public static AssetBundleSettings LoadOrCreate()
        {
            var settings = AssetDatabase.LoadAssetAtPath<AssetBundleSettings>(SettingsPath);
            if (settings != null)
                return settings;

            EnsureFolderExists();
            settings = CreateInstance<AssetBundleSettings>();
            AssetDatabase.CreateAsset(settings, SettingsPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return settings;
        }

        private static void EnsureFolderExists()
        {
            if (AssetDatabase.IsValidFolder("Assets/Editor/AssetBundleSettings"))
                return;
            if (!AssetDatabase.IsValidFolder("Assets/Editor"))
                AssetDatabase.CreateFolder("Assets", "Editor");
            AssetDatabase.CreateFolder("Assets/Editor", "AssetBundleSettings");
        }
    }
}
