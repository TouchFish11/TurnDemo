using System.IO;
using System.Threading.Tasks;
using Core.AssetBundles.Update.Core;
using Core.AssetBundles.Update.Exception;
using Core.Tasks.Extensions;
using Core.Utility;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.AssetBundles.Update.State
{
    /// <summary>
    /// 获取本地目录文件状态类
    /// 负责加载本地资源目录文件（优先读取持久化路径，其次读取StreamingAssets），为后续对比校验做准备
    /// </summary>
    public class LoadLocalCatalogFileState : UpdateState
    {
        /// <summary>
        /// 执行获取本地清单文件核心逻辑
        /// </summary>
        /// <returns>是否获取成功</returns>
        public override async Task<UpdateResult> Execute()
        {
            try
            {
                // 加载本地对比文件
                await GetLocalCompareFileInfo();
            }
            catch (LocalListFileHandleException localListFileHandleException)
            {
                return UpdateResult.CreateFailure(UpdateResult.EUpdateError.LocalListFile, localListFileHandleException);
            }
            catch (System.Exception exception)
            {
                return UpdateResult.CreateFailure(UpdateResult.EUpdateError.Unknown, exception);
            }
            
            return UpdateResult.CreateSuccess();
        }

        /// <summary>
        /// 获取本地AssetBundle对比文件（清单文件）
        /// 优先级：持久化路径 > StreamingAssets路径
        /// </summary>
        /// <returns>是否获取成功</returns>
        public async Task GetLocalCompareFileInfo()
        {
            var persistentListPath = PathUtility.GetAbLoadPath(FileUtility.CatalogDefaultName);
            // 优先读取持久化路径下的清单文件（已更新过的本地清单）
            if (File.Exists(persistentListPath))
            {
                // 使用UnityWebRequest读取（兼容不同平台路径协议）
                await GetLocalCatalog("file:///" + persistentListPath);
                return;
            }
            
            // 读取StreamingAssets路径下的默认清单文件（首次启动/无持久化清单时）
            if (File.Exists(Application.streamingAssetsPath + "/" + FileUtility.CatalogDefaultName))
            {
                // 根据平台拼接路径协议（Android平台StreamingAssets无需file协议）
                var path =
#if UNITY_ANDROID
                    Application.streamingAssetsPath + "/";
#else
                    "file:///" + Application.streamingAssetsPath + "/";
#endif
                await GetLocalCatalog($"{path}{FileUtility.CatalogDefaultName}");
                return;
            }

            // 无本地清单文件（首次启动），直接返回成功（后续对比阶段会处理）
        }

        /// <summary>
        /// 读取指定路径的本地目录并解析
        /// </summary>
        /// <param name="localFilePath">本地清单文件路径（带协议头，如file:///）</param>
        /// <returns>是否读取并解析成功</returns>
        private async Task GetLocalCatalog(string localFilePath)
        {
            // 创建UnityWebRequest请求读取文件
            var req = UnityWebRequest.Get(localFilePath);
            using var handle = req.SendWebRequest().ToTask();
            // 等待请求完成
            await handle.Task;
            // 请求失败，抛出异常
            if (req.result != UnityWebRequest.Result.Success)
            {
                throw new LocalListFileHandleException($"读取本地目录文件失败，{req.result}，{req.error}，{req.responseCode}");
            }
            
            // 解析清单内容到本地包集合
            AnalyzeCatalog(req.downloadHandler.text, EFileAnalyzeType.Local);
            return;
        }

        /// <summary>
        /// 当前更新阶段标识
        /// </summary>
        public override EUpdatePhase UpdatePhase => EUpdatePhase.LoadLocalCatalogFile;
    }
}