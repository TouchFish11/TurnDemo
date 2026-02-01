using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Log;
using Core.Singleton;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Core.Net
{
    /// <summary>
    /// UnityWebRequest������
    /// </summary>
    public class UWRManager : SingletonAutoMono<UWRManager>, IUWRManager
    {
        /// <summary>
        /// �첽������Դ
        /// </summary>
        /// <typeparam name="T">string, byte[], Texture, AudioClip</typeparam>
        /// <param name="path">����(Զ��)·�� ֧�֣�http,ftp,file</param>
        /// <param name="overCallBack">���سɹ��ص�</param>
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
        /// �첽�ϴ���Դ
        /// </summary>
        /// <param name="url">�������ϴ��ӿڵ�ַ</param>
        /// <param name="localPath">�����ļ�·��</param>
        /// <param name="fileName">������������ļ�����null��ʹ��ԭ�ļ�����</param>
        /// <param name="progressCallBack">�ϴ����Ȼص�</param>
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

                // �������
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    progressCallBack?.Invoke(1f);
                }
                else
                {
                    LogManager.LogError($"�ϴ�ʧ��: {uwr.error}\nURL: {url}");
                }
            }
        }
    }
}
