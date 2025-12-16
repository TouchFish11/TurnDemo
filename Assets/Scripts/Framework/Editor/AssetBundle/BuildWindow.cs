using CustomEditor.ScriptGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// AB构建窗口
    /// </summary>
    public class BuildWindow : EditorWindow
    {
        /// <summary>
        /// 平台索引
        /// </summary>
        private int platformIndex = 0;

        /// <summary>
        /// AB包压缩索引
        /// </summary>
        private int compressIndex = 0;

        /// <summary>
        /// 支持的平台数组
        /// </summary>
        private readonly string[] targetPlatformStrs = new string[] { "PC", "IOS", "Android" };

        /// <summary>
        /// AB包压缩模式数组
        /// </summary>
        private readonly string[] compressModeStrs = new string[] { "Uncompress", "LZ4"};

        /// <summary>
        /// 支持的服务器
        /// </summary>
        private string serverIP = "http://...";

        /// <summary>
        /// AB包拷贝路径
        /// </summary>
        private const string AB_COPY_PATH = "Assets/StreamingAssets/AssetBundles/";

        /// <summary>
        /// 用户名
        /// </summary>
        private string userName = "userName";

        /// <summary>
        /// 密码
        /// </summary>
        private string password = "password";

        /// <summary>
        /// 每帧最大上传字节数
        /// </summary>
        private uint maxBytesCapacity = 4096;

        /// <summary>
        /// 资源输入路径
        /// </summary>
        private const string AssetsInputPath = "Assets/Editor/ArtRes/";

        /// <summary>
        /// AssetBundle输出路径
        /// </summary>
        private const string AssetBundlesOutPath = "Assets/AssetBundleDatas/";

        /// <summary>
        /// 当前上传最大数量
        /// </summary>
        private static int upLoadmaxNum;

        /// <summary>
        /// 当前上传成功数量
        /// </summary>
        private static int nowUpLoadFinishedNum;

        /// <summary>
        /// 滚动条位置
        /// </summary>
        private Vector2 pos;

        /// <summary>
        /// AB包打包压缩模式
        /// </summary>
        private BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.None;

        // AB包打包设置
        private bool isAppendHashToAssetBundleName = false;
        private bool isUseContentHash = false;
        private bool isIgnoreTypeTreeChanges = false;
        private bool isDryRunBuild = false;
        private bool isEnableProtection = false;
        private bool isStrictMode = false;

        /// <summary>
        /// 是否展开折叠
        /// </summary>
        private bool isFoldExpand = true;

        /// <summary>
        /// 上传时是否需要输入账号密码
        /// </summary>
        private bool uploadUseUser = false;

        /// <summary>
        /// 密码是否可见
        /// </summary>
        private bool showPassWord = false;

        //每次最大上传字节数是否自动设置
        private bool uploadBytesIsAutoSetting = true;
        private bool uploadBytesIsCustomSetting = false;

        // 文件信息字典  根文件的子文件夹<—>文件列表
        private readonly Dictionary<string, List<FileInfo>> _fileInfoDic = new Dictionary<string, List<FileInfo>>();

        // 文件过滤后缀数组
        private readonly string[] _filterSuffixes = new string[] { ".meta" };

        // 预定义枚举数组
        private readonly string[] defaultNames = new string[] { "UI", "Scene", "Lua", "Music", "Json", "TableInfo", "InputData", "Camera" };

        // 枚举文件生成路径
        private readonly string filePath = Application.dataPath + "/Scripts/Framework/AssetBundle/Management/E_AssetBundleType.cs";

        [MenuItem("GameTool/AssetBundle/Build Package Window")]
        private static void OpenWindow()
        {
            BuildWindow window = EditorWindow.GetWindow<BuildWindow>("Build Window");
            window.Show();
        }

        private void OnGUI()
        {
            pos = GUILayout.BeginScrollView(pos);

            GUILayout.Label(new GUIContent("AssetsInputPath", "将被打包成AB包的资源本地存储默认路径"));

            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(AssetsInputPath);
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button(new GUIContent("Create Path", "创建资源存储路径")))
            {
                if (!Directory.Exists(AssetsInputPath))
                {
                    Directory.CreateDirectory(AssetsInputPath);
                    AssetDatabase.Refresh();
                    Debug.Log($"生成路径成功：{AssetsInputPath}");
                }
            }

            GUILayout.Label(new GUIContent("TargetPlatform", "平台选择"));

            platformIndex = GUILayout.Toolbar(platformIndex, targetPlatformStrs);

            GUILayout.Label(new GUIContent("AssetBundlesOutPath", "AB包本地存储默认路径"));

            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(AssetBundlesOutPath + targetPlatformStrs[platformIndex]);
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button(new GUIContent("Create Path", "创建AB包存储路径")))
            {
                if (!Directory.Exists(AssetBundlesOutPath + targetPlatformStrs[platformIndex]))
                {
                    Directory.CreateDirectory(AssetBundlesOutPath + targetPlatformStrs[platformIndex]);
                    AssetDatabase.Refresh();
                    Debug.Log($"生成路径成功：{AssetBundlesOutPath + targetPlatformStrs[platformIndex]}");
                }
            }

            GUILayout.Label(new GUIContent("ServerAddress", "服务器地址"));
            serverIP = GUILayout.TextField(serverIP);

            GUILayout.Label(new GUIContent("AssetBundleConfigPath", "AB包配置文件"));

            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(MenuItemTools.AssetPath + "AssetBundlesCollections.asset");
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button(new GUIContent("Create Config", "创建AB包配置文件")))
            {
                CreateCollection();
            }
            if (GUILayout.Button(new GUIContent("Set AssetBundleLabel", "设置资源AB包标签")))
            {
                EditAssetLabel();
            }

            GUILayout.Label(new GUIContent("AssetBundlePath", "AB存储路径"));

            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(AssetBundlesOutPath + targetPlatformStrs[platformIndex]);
            EditorGUI.EndDisabledGroup();

            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;

            isFoldExpand = EditorGUILayout.Foldout(isFoldExpand, new GUIContent("BuildConfig", "构建配置"), true);
            if(isFoldExpand)
            {
                GUILayout.Label(new GUIContent("Compressed Mode", "压缩模式"));
                compressIndex = GUILayout.Toolbar(compressIndex, new GUIContent[] {new GUIContent(compressModeStrs[0], "不压缩"), new GUIContent(compressModeStrs[1], "块压缩") });

                isAppendHashToAssetBundleName = GUILayout.Toggle(isAppendHashToAssetBundleName, new GUIContent("AppendHashToAssetBundleName", "在AB包名后追加哈希"));
                isUseContentHash = GUILayout.Toggle(isUseContentHash, new GUIContent("UseContentHash", "使用包内容计算哈希"));

                isIgnoreTypeTreeChanges = GUILayout.Toggle(isIgnoreTypeTreeChanges, new GUIContent("IgnoreTypeTreeChanges", "忽略类型树变化"));

                isDryRunBuild = GUILayout.Toggle(isDryRunBuild, new GUIContent("DryRunBuild", "模拟构建"));

                isEnableProtection = GUILayout.Toggle(isEnableProtection, new GUIContent("EnableProtection", "启用AB包加密"));

                isStrictMode = GUILayout.Toggle(isStrictMode, new GUIContent("Strict Mode", "严格模式"));
            }

            if (GUILayout.Button(new GUIContent("Build AssetBundles", "构建AB包")))
            {
                BuildAssetBundles();
            }

            GUILayout.Label(new GUIContent("ABListFilePath", "AB包清单文件路径"));

            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(AssetBundlesOutPath + targetPlatformStrs[platformIndex] + "/" + "AssetBundleListInfo.json");
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button(new GUIContent("Create ABListFile", "创建AB包清单文件")))
            {
                CreateAssetBundleListFile(AssetBundlesOutPath, AssetBundlesOutPath + targetPlatformStrs[platformIndex], $"{AssetBundlesOutPath}{targetPlatformStrs[platformIndex]}/{FileUtility.ListFileDefaultName}");
            }

            GUILayout.Label(new GUIContent("AssetBundleTransferPath", "AB包转存路径"));

            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(AB_COPY_PATH);
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button(new GUIContent("Move AssetBundle To StreamingAssets", "拷贝AssetBundle到流文件夹")))
            {
                MoveABToStreamingAssets();
            }

            GUILayout.Label(new GUIContent("PreviewServerIP", "预览服务器地址"));

            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(serverIP);
            EditorGUI.EndDisabledGroup();

            uploadUseUser = GUILayout.Toggle(uploadUseUser, new GUIContent("Verify Identity", "是否在上传时进行身份验证"));

            if (uploadUseUser)
            {
                GUILayout.Label(new GUIContent("UserName", "用户名"));
                userName = GUILayout.TextField(userName);

                showPassWord = GUILayout.Toggle(showPassWord, new GUIContent("Show Password", "显示/隐藏密码"));

                GUILayout.Label(new GUIContent("Password", "密码"));
                if (showPassWord)
                    password = GUILayout.TextField(password);
                else
                    password = GUILayout.PasswordField(password, '*');
            }

            GUILayout.Label(new GUIContent("Max UpLoad-Bytes Per One Time", "每次上传最大字节数"));

            uploadBytesIsCustomSetting = GUILayout.Toggle(!uploadBytesIsAutoSetting, new GUIContent("Custom", "手动设置"));
            uploadBytesIsAutoSetting = GUILayout.Toggle(!uploadBytesIsCustomSetting, new GUIContent("Auto", "自动设置"));

            //自定义字节数
            if (!uploadBytesIsAutoSetting)
            {
                GUILayout.Label(new GUIContent("Custom Max UpLoad-Bytes Per One Time", "自定义每次最大上传字节数"));
                maxBytesCapacity = uint.Parse(GUILayout.TextField(maxBytesCapacity.ToString()), System.Globalization.NumberStyles.Number);
            }

            if (GUILayout.Button(new GUIContent("Upload AssetBundleDatas", "上传AB包数据")))
            {
                UpLoadAssetBundlesData();
            }

            GUILayout.EndScrollView();
        }

        /// <summary>
        /// 创建AB包配置
        /// </summary>
        private void CreateCollection()
        {
            //判断是否存在该文件夹
            //不存在则创建
            if (!Directory.Exists(MenuItemTools.AssetPath))
            {
                Directory.CreateDirectory(MenuItemTools.AssetPath);
                Debug.Log($"该路径不存在：{MenuItemTools.AssetPath}，已自动创建路径！");
            }

            //创建ScriptableObject实例
            AssetBundlesCollections collections = ScriptableObject.CreateInstance<AssetBundlesCollections>();
            //创建配置文件
            AssetDatabase.CreateAsset(collections, $"{MenuItemTools.AssetPath}AssetBundlesCollections.asset");
            //保存文件
            AssetDatabase.SaveAssets();
            //刷新
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 编辑资源AB包标签
        /// </summary>
        private void EditAssetLabel()
        {
            if (!Directory.Exists(AssetsInputPath))
            {
                Debug.Log($"该路径不存在：{AssetsInputPath}，已自动创建路径");
                Directory.CreateDirectory(AssetsInputPath);
                return;
            }

            //获取根文件夹
            DirectoryInfo directoryInfo = Directory.CreateDirectory(AssetsInputPath);

            AssetBundlesCollections.Instance.Clear();
            _fileInfoDic.Clear();

            //遍历所有的文件夹
            DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();

            for (int i = 0; i < directoryInfos.Length; i++)
            {
                //获取其中一个文件夹下的所有文件
                List<FileInfo> fileInfos = FileUtility.GetTotalFiles(directoryInfos[i], new List<FileInfo>(), _filterSuffixes);
                //存储文件列表
                _fileInfoDic.Add(directoryInfos[i].Name, fileInfos);
            }

            //遍历文件信息字典
            foreach (string abName in _fileInfoDic.Keys)
            {
                List<FileInfo> fileInfos = _fileInfoDic[abName];
                for (int i = 0; i < fileInfos.Count; i++)
                {
                    //工程资源路径
                    string dataPath = fileInfos[i].FullName[fileInfos[i].FullName.IndexOf("Assets")..];
                    AssetImporter importer = AssetImporter.GetAtPath(dataPath);
                    if (importer != null)
                    {
                        //设置该文件的AB包名
                        importer.assetBundleName = abName.ToLower();
                        //记录到AB包收集类中
                        AssetBundlesCollections.Instance.Add(importer.assetBundleName, new AssetBundlesCollections.AssetInfo(dataPath, fileInfos[i].Length, fileInfos[i].Name));
                    }
                }
            }

            IScriptGenerator scriptGenerator = new EnumGenerator(_fileInfoDic.Keys, defaultNames, filePath, "Framework");
            scriptGenerator.GenerateScript();
        }

        /// <summary>
        /// 构建AB包
        /// </summary>
        private void BuildAssetBundles()
        {
            //判断是否存在输出路径
            if (!Directory.Exists(AssetBundlesOutPath + targetPlatformStrs[platformIndex]))
            {
                Debug.Log($"该路径{AssetBundlesOutPath + targetPlatformStrs[platformIndex]}不存在，请先创建路径！");
                return;
            }

            //若该文件夹下没有资源则不处理
            if (Directory.CreateDirectory(AssetsInputPath).GetFiles().Length == 0)
            {
                Debug.Log("没有可用于构建为AssetBunlde的资源");
                return;
            }

            DirectoryInfo info = Directory.CreateDirectory(AssetBundlesOutPath + targetPlatformStrs[platformIndex]);
            FileInfo[] infos = info.GetFiles();
            for (int i = 0; i < infos.Length; i++)
            {
                File.Delete(infos[i].FullName);
            }

            //设置打包配置
            /*  未压缩。
                构建AssetBundle时，AssetBundle文件不会进行压缩处理。这意味着AssetBundle文件将以原始的、未压缩的形式存储资源数据。加载速度快但文件体积大。
             */
            if (compressModeStrs[compressIndex] == "Uncompress")
                assetBundleOptions = BuildAssetBundleOptions.UncompressedAssetBundle;
            /*  LZ4压缩。
                采用基于块的压缩算法对 AssetBundle 进行压缩。它会将 AssetBundle 分割成多个小块，然后对每个小块分别进行压缩。较好的压缩比，部分加载效率高但加载速度相对较慢。
             */
            else if (compressModeStrs[compressIndex] == "LZ4")
                assetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;

            /*  在资源包名称后面追加哈希值。
                在 AssetBundle 文件名后面追加基于其内容生成的哈希值。这样做的好处在于方便对 AssetBundle 进行版本管理和更新操作。
                当 AssetBundle 的内容发生改变时，其对应的哈希值也会改变，文件名随之改变。客户端可以通过比较文件名中的哈希值，
                快速判断本地的AssetBundle是否为最新版本，从而决定是否需要下载更新。
             */
            if (isAppendHashToAssetBundleName)
                assetBundleOptions |= BuildAssetBundleOptions.AppendHashToAssetBundleName;

            /*  使用内容哈希。
                依据AssetBundle中资源的内容来生成哈希值。若资源内容未发生改变，生成的哈希值也不会变。有效管理 AssetBundle 的缓存，避免不必要的重新下载和更新。
                客户端可以通过比较哈希值来判断是否需要更新本地的 AssetBundle 文件。
            */
            if (isUseContentHash)
                assetBundleOptions |= BuildAssetBundleOptions.UseContentHash;

            /*
                忽略类型树变化。
                当脚本的类结构发生变化（比如添加、删除字段或修改字段类型），类型树也会随之改变。使用该选项，只要资源的实际二进制数据未变，
                即使类型树有改变，Unity 也不会重新构建 AssetBundle。这在项目开发过程中，特别是脚本频繁修改但资源内容未实质改变时，能节省大量的构建时间。
             */
            if (isIgnoreTypeTreeChanges)
                assetBundleOptions |= BuildAssetBundleOptions.IgnoreTypeTreeChanges;

            /*  模拟构建。
                进行模拟的AssetBundle构建过程，而不会实际生成 AssetBundle 文件。这意味着在执行构建操作时，Unity 会对资源进行分析，
                确定哪些资源会被包含在AssetBundle中、计算资源的依赖关系以及估计构建所需的时间和资源等，但不会将这些资源打包成最终的AssetBundle文件。
                可以提前了解构建过程的复杂程度和所需资源，从而合理安排构建计划。
            */
            if (isDryRunBuild)
                assetBundleOptions |= BuildAssetBundleOptions.DryRunBuild;


            /*  启用资源包保护。
                对构建的AssetBundle进行加密保护。这能防止AssetBundle被轻易逆向工程，避免他人未经授权就访问和篡改其中的资源，有助于保护开发者的知识产权和商业利益。
                应用程序在运行时加载受保护的 AssetBundle 时，Unity 会自动对其进行解密，使应用程序能够正常使用其中的资源。这个解密过程对开发者来说是透明的，开发者无需手动处理。
            */
            if (isEnableProtection)
                assetBundleOptions |= BuildAssetBundleOptions.EnableProtection;

            /*  严格模式。
                对构建过程进行严格的错误检查。一旦在构建过程中发现任何问题，比如资源引用错误、资源丢失、序列化问题等，构建过程就会立即停止，
                并抛出相应的错误信息。这与非严格模式不同，在非严格模式下，Unity可能会尝试忽略一些小问题继续完成构建，但这可能会导致最终生成的AssetBundle在运行时出现难以调试的问题。
                在项目开发的早期阶段，使用严格模式可以帮助开发者及时发现并解决资源管理和配置方面的问题，避免问题积累到后期导致更复杂的错误。
            */
            if (isStrictMode)
                assetBundleOptions |= BuildAssetBundleOptions.StrictMode;

            //目标平台
            BuildTarget buildTarget = BuildTarget.NoTarget;
            switch (targetPlatformStrs[platformIndex])
            {
                case "PC":
                    buildTarget = BuildTarget.StandaloneWindows;
                    break;
                case "IOS":
                    buildTarget = BuildTarget.iOS;
                    break;
                case "Android":
                    buildTarget = BuildTarget.Android;
                    break;
            }

            //构建AB包
            BuildPipeline.BuildAssetBundles(AssetBundlesOutPath + targetPlatformStrs[platformIndex], assetBundleOptions, buildTarget);

            //刷新
            AssetDatabase.Refresh();

            //获取存储AB包文件夹下的所有文件
            DirectoryInfo directoryInfo = Directory.CreateDirectory(AssetBundlesOutPath + targetPlatformStrs[platformIndex]);
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            for (int i = 0; i < fileInfos.Length; i++)
            {
                //排除后缀非AB包后缀的文件
                if (fileInfos[i].Extension == ".manifest" || fileInfos[i].Extension == ".meta")
                {
                    //删除.meta后缀的文件,之后会自动重新生成
                    if (fileInfos[i].Extension == ".meta")
                        File.Delete(fileInfos[i].FullName);
                    continue;
                }
                //将默认的AB包后缀改为自定义后缀
                string newFileName = Path.ChangeExtension(fileInfos[i].FullName, ".assetbundle");

                //如果目标文件存在,先删除目标文件,再移动,避免报错
                if (File.Exists(newFileName))
                    File.Delete(newFileName);

                //覆盖原来的AB包文件
                File.Move(fileInfos[i].FullName, newFileName);
            }

            //刷新
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 创建AB包清单文件
        /// </summary>
        private void CreateAssetBundleListFile(string outPath, string dirctoryPath, string filePath)
        {
            // 获取AssetBundle输出路径下的所有AB包
            if (!Directory.Exists(outPath))
            {
                Directory.CreateDirectory(outPath);
            }

            // 获取AB包中的数据
            DirectoryInfo directoryInfo = Directory.CreateDirectory(dirctoryPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles();

            // 创建包集合
            ABPackageCollection collection = new ABPackageCollection();
            foreach (FileInfo info in fileInfos)
            {
                if (info.Extension != AssetBundleManager.Instance.AbSuffix)
                {
                    continue;
                }

                // 构建每个包的信息
                ABPackageInfo packageInfo = new ABPackageInfo(info.Name, info.Length, GetMD5(info.FullName));
                // 记录包信息
                collection.TryAdd(info.Name, packageInfo);
            }

            // Json序列化
            JsonManager.Instance.ToJson(collection, filePath, E_JsonType.JsonUtlity);
            // 刷新
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 将选中资源复制到StreamingAssets中
        /// </summary>
        private void MoveABToStreamingAssets()
        {
            if (!Directory.Exists(AB_COPY_PATH))
            {
                Directory.CreateDirectory(AB_COPY_PATH);
                AssetDatabase.Refresh();
                Debug.Log($"路径：{AB_COPY_PATH}不存在，已自动创建");
            }

            //获取在Project窗口中选中的资源
            System.Object[] selAssets = Selection.GetFiltered(typeof(System.Object), SelectionMode.DeepAssets);

            if (selAssets.Length == 0)
                return;

            for (int i = 0; i < selAssets.Length; i++)
            {
                string assetPath = AssetDatabase.GetAssetPath((UnityEngine.Object)selAssets[i]);
                string fileName = assetPath[(assetPath.LastIndexOf('/') + 1)..];
                if (fileName.IndexOf(".assetbundle") == -1)
                    continue;

                AssetDatabase.CopyAsset(assetPath, AB_COPY_PATH + "/" + fileName);
            }

            CreateAssetBundleListFile(AB_COPY_PATH, AB_COPY_PATH, $"{AB_COPY_PATH}{FileUtility.ListFileDefaultName}");
        }

        /// <summary>
        /// 上传AB包数据
        /// </summary>
        private void UpLoadAssetBundlesData()
        {
            //获取AB包输出路径下的AB包数据
            DirectoryInfo directory = Directory.CreateDirectory(AssetBundlesOutPath + targetPlatformStrs[platformIndex]);
            //获取所有文件数据
            FileInfo[] fileInfos = directory.GetFiles();
            //声明临时变量记录需要上传的文件
            List<FileInfo> list = new List<FileInfo>();
            //获取需要上传的文件
            for (int i = 0; i < fileInfos.Length; i++)
            {
                if (fileInfos[i].Extension != AssetBundleManager.Instance.AbSuffix && fileInfos[i].Extension != ".json")
                    continue;

                list.Add(fileInfos[i]);
            }

            //上传总数
            upLoadmaxNum = list.Count;
            //当前已上传数
            nowUpLoadFinishedNum = 0;
            //上传文件
            for (int i = 0; i < list.Count; i++)
            {
                UpLoadHttp(list[i].FullName, list[i].Name);
            }

            //HTTP上传（异步函数）
            async void UpLoadHttp(string filePath, string fileName)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        //创建HttpWebRequest对象
                        HttpWebRequest req = HttpWebRequest.Create(new Uri(serverIP)) as HttpWebRequest;
                        //设置请求类型，内容类型，超时，身份验证
                        req.Method = WebRequestMethods.Http.Post;
                        req.ContentType = "multipart/form-data;boundary=MrQiu";
                        req.Timeout = 500000;
                        if(uploadUseUser)
                            req.Credentials = new NetworkCredential(userName, password);
                        req.PreAuthenticate = true;  //先验证身份，再上传数据
                                                     //按照格式拼接字符串转为字节数组用于上传
                        string head = "--MrQiu\r\n" +
                            $"Content-Disposition:form-data;name=\"file\";filename=\"{fileName}\"\r\n" +
                            "Content-Type:application/octet-stream\r\n\r\n";
                        byte[] headBytes = Encoding.UTF8.GetBytes(head);
                        byte[] endBytes = Encoding.UTF8.GetBytes("\r\n--MrQiu--\r\n");
                        //写入上传流
                        using (FileStream fileStream = File.OpenRead(filePath))
                        {
                            //总长度=前部分 + 文件本身 + 后部分
                            req.ContentLength = headBytes.Length + fileStream.Length + endBytes.Length;

                            //自动长度
                            long autoLength = 0;
                            //上传自动设置最大字节数
                            if (uploadBytesIsAutoSetting)
                            {
                                //长度：>=100MB
                                if (req.ContentLength >= 1024 * 1024 * 100)
                                {
                                    autoLength = 1024 * 1024;
                                }
                                //长度：50~100MB
                                else if (req.ContentLength >= 1024 * 1024 * 50 && req.ContentLength < 1024 * 1024 * 100)
                                {
                                    autoLength = 65536;
                                }
                                //长度：1~50MB
                                else if (req.ContentLength > 1024 * 1024 && req.ContentLength < 1024 * 1024 * 50)
                                {
                                    autoLength = 4096;
                                }
                                //长度：<=1MB
                                else
                                {
                                    autoLength = req.ContentLength;
                                }
                            }

                            //用于上传的流
                            Stream upLoadStream = req.GetRequestStream();
                            upLoadStream.Write(headBytes, 0, headBytes.Length);
                            byte[] bytes = new byte[uploadBytesIsAutoSetting ? autoLength : maxBytesCapacity];
                            int length = fileStream.Read(bytes, 0, bytes.Length);
                            while (length != 0)
                            {
                                upLoadStream.Write(bytes, 0, length);
                                length = fileStream.Read(bytes, 0, bytes.Length);
                            }
                            upLoadStream.Write(endBytes, 0, endBytes.Length);
                            upLoadStream.Close();
                            fileStream.Close();
                        }
                        HttpWebResponse res = req.GetResponse() as HttpWebResponse;
                        res.Close();
                        if (res.StatusCode == HttpStatusCode.OK)
                        {
                            Stream stream = res.GetResponseStream();
                            using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
                            {
                                string responseText = sr.ReadToEnd();
                                if(responseText != "")
                                    Debug.Log($"服务器响应信息：{responseText}");
                            }

                            Debug.Log($"{fileName}上传成功, 当前进度：{++nowUpLoadFinishedNum}/{upLoadmaxNum}");
                        }
                        else
                        {
                            Debug.Log($"{fileName}上传失败" + res.StatusCode);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                });
            }
        }

        /// <summary>
        /// 获取MD5码
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string GetMD5(string filePath)
        {
            using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            //声明一个MD5对象，用于生成MD5码
            MD5 md5 = new MD5CryptoServiceProvider();
            //计算MD5码
            byte[] md5Info = md5.ComputeHash(fileStream);
            //关闭流文件
            fileStream.Close();

            StringBuilder sb = new StringBuilder();
            //将16个字节的MD5码转为16进制，为了减小MD5码的长度
            for (int i = 0; i < md5Info.Length; i++)
            {
                sb.Append(md5Info[i].ToString("x2"));
            }

            return sb.ToString();
        }

        ///// <summary>
        ///// 获取所有文件
        ///// </summary>
        ///// <param name="directoryInfo"></param>
        ///// <param name="fileInfos"></param>
        ///// <returns></returns>
        //private List<FileInfo> GetTotalFiles(DirectoryInfo directoryInfo, List<FileInfo> fileInfos)
        //{
        //    //获取并存储当前文件夹的所有文件
        //    List<FileInfo> temps = directoryInfo.GetFiles().ToList();
        //    for (int i = temps.Count - 1; i >= 0; i--)
        //    {
        //        if (_filterSuffixes.Contains(temps[i].Extension))
        //        {
        //            temps.RemoveAt(i);
        //        }
        //    }

        //    fileInfos.AddRange(temps);
        //    //获取下一级的所有子文件夹
        //    DirectoryInfo[] subDirectoryInfos = directoryInfo.GetDirectories();
        //    //存储该级的所有子文件夹信息
        //    foreach (DirectoryInfo info in subDirectoryInfos)
        //    {
        //        GetTotalFiles(info, fileInfos);
        //    }
        //    return fileInfos;
        //}
    }
}
