using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework
{
    /// <summary>
    /// 获取本地清单文件状态
    /// </summary>
    public class GetLocalListFileState : UpdateState
    {
        public GetLocalListFileState(AssetBundleUpdater updater) : base(updater)
        {

        }

        public override async Task<bool> Execute()
        {
            // 获取本地对比文件
            IsSuceess = await GetLocalCompareFileInfo();
            if (!IsSuceess)
            {
                LogManager.LogError("本地对比文件获取失败");
                FinishUpdate();
                return IsSuceess;
            }

            // 切换至对比差异状态
            assetBundleUpdater.ChangeState(E_UpdatePhase.CompareContrast);
            return IsSuceess;
        }

        /// <summary>
        /// 读取本地AB包对比文件
        /// </summary>
        /// <param name="overCallBack">读取结束回调</param>
        public async Task<bool> GetLocalCompareFileInfo()
        {
            // 可读写路径有本地对比文件，说明已经更新过了，通过UnityWebRequest获取本地的对比文件
            if (File.Exists(PathManager.GetAbLoadPath(FileUtility.ListFileDefaultName)))
            {
                // 通过UnityWebRequest获取本地可读写路径的对比文件需要添加文件协议
                return await GetLocaListFileInfo("file:///" + PathManager.GetAbLoadPath(FileUtility.ListFileDefaultName));
            }
            // 流文件夹有对比文件，说明是有默认资源且是第一次更新，通过UnityWebRequest获取本地的对比文件
            else if (File.Exists(Application.streamingAssetsPath + "/" + FileUtility.ListFileDefaultName))
            {
                // 根据不同的平台判断是否需要添加文件协议
                string path =
#if UNITY_ANDROID
                    Application.streamingAssetsPath + "/";
#else
                    "file:///" + Application.streamingAssetsPath + "/";
#endif
                return await GetLocaListFileInfo(path + FileUtility.ListFileDefaultName);
            }
            else
            {
                // 说明没有默认资源，且是第一次更新，不用获取
                return true;
            }
        }

        /// <summary>
        /// 获取本地清单文件信息
        /// </summary>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        private async Task<bool> GetLocaListFileInfo(string localFilePath)
        {
            // 获取本地AB包对比文件
            UnityWebRequest req = UnityWebRequest.Get(localFilePath);

            await WaitForTask(req.SendWebRequest());

            // 获取成功才去解析
            if (req.result == UnityWebRequest.Result.Success)
            {
                //解析本地AB包对比文件
                AnalyzeCompareFileInfo(req.downloadHandler.text, E_FileAnalyzeType.Local);
                return true;
            }
            else
            {
                LogManager.LogError($"本地AB包对比文件获取失败：{req.result}-{req.error}");
                //获取失败
                return false;
            }
        }

        public static Task WaitForTask(AsyncOperation asyncOperation)
        {
            TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();
            asyncOperation.completed += ao => source.SetResult(true);
            return source.Task;
        }

        public override E_UpdatePhase UpdatePhase => E_UpdatePhase.GetLocalCompareFile;
    }
}
