using System.Collections;
using System.Collections.Generic;
using System.IO;
using Core.DI;
using Core.Mono;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Logger = Core.Log.Logger;

namespace Core.Net
{
    /// <summary>
    /// UnityWebRequest管理类
    /// 封装网络请求相关操作：资源下载、文件上传
    /// </summary>
    public class UWRManager : IUWRManager
    {
        // Mono适配器
        private readonly IMonoAdapter _monoAdapter;

        private UWRManager(IMonoAdapter monoAdapter)
        {
            _monoAdapter = monoAdapter;
        }

        /// <summary>
        /// 异步加载/下载资源
        /// </summary>
        /// <typeparam name="T">支持类型：string, byte[], Texture, AudioClip</typeparam>
        /// <param name="path">资源路径（支持http/ftp/file协议）</param>
        /// <param name="overCallBack">请求完成回调（参数1：是否成功，参数2：下载的资源）</param>
        public void LoadAssetAsync<T>(string path, UnityAction<bool, T> overCallBack) where T : class
        {
            // 通过Mono适配器启动下载协程
            _monoAdapter.StartCoroutine(LoadAssetAsync_Cor(path, overCallBack));
            return;

            // 下载资源核心协程方法
            static IEnumerator LoadAssetAsync_Cor(string path, UnityAction<bool, T> overCallBack)
            {
                UnityWebRequest req;

                // 根据返回类型创建对应类型的请求
                if (typeof(T) == typeof(string) || typeof(T) == typeof(byte[]))
                    req = UnityWebRequest.Get(path); // 文本/字节数组请求
                else if(typeof(T) == typeof(Texture))
                    req = UnityWebRequestTexture.GetTexture(path); // 纹理资源请求
                else if(typeof(T) == typeof(AudioClip))
                    req = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG); // 音频资源请求
                else
                {
                    // 不支持的类型，回调返回失败
                    overCallBack?.Invoke(false, null);
                    yield break;
                }

                // 发送请求并等待完成
                yield return req.SendWebRequest();

                // 请求成功：根据类型返回对应数据
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
                    // 请求失败：回调返回null
                    overCallBack?.Invoke(false, null);

                // 释放请求资源
                req.Dispose();
            }
        }

        /// <summary>
        /// 异步上传资源文件
        /// </summary>
        /// <param name="url">上传接口地址</param>
        /// <param name="localPath">本地文件路径</param>
        /// <param name="fileName">自定义文件名（null则使用原文件名）</param>
        /// <param name="progressCallBack">上传进度回调（0~1的进度值）</param>
        public void UploadAssetAsync(string url, string localPath, string fileName = null, UploadProgressCallBack progressCallBack = null)
        {
            // 通过Mono适配器启动上传协程
            _monoAdapter.StartCoroutine(UploadAssetAsync_Cor());
            return;

            // 上传文件核心协程方法
            IEnumerator UploadAssetAsync_Cor()
            {
                // 异步读取本地文件字节数据（避免主线程阻塞）
                var task = File.ReadAllBytesAsync(localPath);

                // 等待文件读取完成
                yield return new WaitUntil(() => task.IsCompleted);

                // 文件读取失败：打印错误并终止
                if (!task.IsCompletedSuccessfully)
                {
                    Logger.LogError(task.Exception.Message);
                    yield break;
                }

                // 构建表单数据（用于POST上传）
                var dataList = new List<IMultipartFormSection>()
                {
                    new MultipartFormDataSection(fileName ?? Path.GetFileName(localPath), task.Result)
                };

                // 创建POST请求（using自动释放资源）
                using var uwr = UnityWebRequest.Post(url, dataList);

                // 发送上传请求
                uwr.SendWebRequest();

                // 循环回调上传进度
                while (!uwr.isDone)
                {
                    progressCallBack?.Invoke(uwr.uploadProgress);
                    yield return null;
                }

                // 上传完成处理
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    // 上传成功：进度回调100%
                    progressCallBack?.Invoke(1f);
                }
                else
                {
                    // 上传失败：打印错误信息
                    Logger.LogError($"上传失败: {uwr.error}\nURL: {url}");
                }
            }
        }
    }
}