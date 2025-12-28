using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Framework
{
    /// <summary>
    /// 上传进度回调
    /// </summary>
    /// <param name="progress">当前进度（0-1）</param>
    public delegate void UploadProgressCallBack(float progress);

    /// <summary>
    /// UnityWebRequest管理器
    /// </summary>
    public class UWRManager : SingletonAutoMono<UWRManager>, IUWRManager
    {
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">string, byte[], Texture, AudioClip</typeparam>
        /// <param name="path">本地(远程)路径 支持；http,ftp,file</param>
        /// <param name="overCallBack">加载成功回调</param>
        public void LoadAssetAsync<T>(string path, UnityAction<bool, T> overCallBack) where T : class
        {
            StartCoroutine(LoadAssetAsync_Cor(path, overCallBack));

            static IEnumerator LoadAssetAsync_Cor(string path, UnityAction<bool, T> overCallBack)
            {
                UnityWebRequest req;

                if (typeof(T) == typeof(string) || typeof(T) == typeof(byte[]))
                    req = UnityWebRequest.Get(path);
                else if(typeof(T) == typeof(Texture))
                    req = UnityWebRequestTexture.GetTexture(path);
                else if(typeof(T) == typeof(AudioClip))
                    req = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG);
                else
                {
                    overCallBack?.Invoke(false, null);
                    yield break;
                }

                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    if (typeof(T) == typeof(string))
                        overCallBack?.Invoke(true, req.downloadHandler.text as T);
                    else if (typeof(T) == typeof(byte[]))
                        overCallBack?.Invoke(true, req.downloadHandler.data as T);
                    else if (typeof(T) == typeof(Texture))
                        overCallBack?.Invoke(true, DownloadHandlerTexture.GetContent(req) as T);
                    else if (typeof(T) == typeof(AudioClip))
                        overCallBack?.Invoke(true, DownloadHandlerAudioClip.GetContent(req) as T);
                }
                else
                    overCallBack?.Invoke(false, null);

                req.Dispose();
            }
        }

        /// <summary>
        /// 异步上传资源
        /// </summary>
        /// <param name="url">服务器上传接口地址</param>
        /// <param name="localPath">本地文件路径</param>
        /// <param name="fileName">服务器保存的文件名（null则使用原文件名）</param>
        /// <param name="progressCallBack">上传进度回调</param>
        public void UploadAssetAsync(string url, string localPath, string fileName = null, UploadProgressCallBack progressCallBack = null)
        {
            StartCoroutine(UploadAssetAsync_Cor());

            IEnumerator UploadAssetAsync_Cor()
            {
                Task<byte[]> task = File.ReadAllBytesAsync(localPath);

                yield return new WaitUntil(() => task.IsCompleted);

                if (!task.IsCompletedSuccessfully)
                {
                    LogManager.LogError(task.Exception.Message);
                    yield break;
                }

                List<IMultipartFormSection> dataList = new List<IMultipartFormSection>()
                {
                    new MultipartFormDataSection(fileName ?? Path.GetFileName(localPath), task.Result)
                };

                using UnityWebRequest uwr = UnityWebRequest.Post(url, dataList);

                uwr.SendWebRequest();

                while (!uwr.isDone)
                {
                    progressCallBack?.Invoke(uwr.uploadProgress);
                    yield return null;
                }

                // 处理结果
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    progressCallBack?.Invoke(1f);
                }
                else
                {
                    LogManager.LogError($"上传失败: {uwr.error}\nURL: {url}");
                }
            }
        }
    }
}
