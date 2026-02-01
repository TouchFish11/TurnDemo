using System.Collections;
using System.Collections.Generic;
using Core.Mono;
using Core.Singleton;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Res
{
    /// <summary>
    /// Resources��Դ������
    /// </summary>
    public class ResourcesManager : SingletonBase<ResourcesManager>, IResourcesManager
    {
        // ��Դ������Դ��Ϣӳ��
        private readonly Dictionary<string, BaseResourcesInfo> _nameToResInfoMap = new Dictionary<string, BaseResourcesInfo>();

        private ResourcesManager()
        {

        }

        /// <summary>
        /// ͬ��������Դ
        /// </summary>
        /// <typeparam name="T">��Դ����</typeparam>
        /// <param name="resPath">��Դ·��</param>
        /// <returns></returns>
        public T Load<T>(string resPath) where T : Object
        {
            //�Զ���洢����
            string cacheName = $"{resPath}_{typeof(T).Name}";
            ResourcesInfo<T> info = null;
            if (_nameToResInfoMap.ContainsKey(cacheName))
            {
                info = _nameToResInfoMap[cacheName] as ResourcesInfo<T>;
                if (info.Asset == null)
                {
                    MonoManager.Instance.StopCoroutine(info.ResCoroutine);
                    //�ÿ�Э��
                    info.ResCoroutine = null;
                    //ͬ�����أ���¼��Դ
                    info.Asset = Resources.Load<T>(resPath);
                    //ִ�лص�
                    info.Invoke();
                    return info.Asset;
                }
                else
                {
                    return info.Asset;
                }
            }

            info = new ResourcesInfo<T>(null);
            //�洢���ֵ���
            _nameToResInfoMap.Add(cacheName, info);
            //ͬ�����أ���¼��Դ
            info.Asset = Resources.Load<T>(resPath);
            return info.Asset;
        }

        /// <summary>
        /// �첽������Դ
        /// </summary>
        /// <typeparam name="T">��Դ����</typeparam>
        /// <param name="resName">��Դ·��</param>
        /// <param name="callBack">�ص�����</param>
        public void LoadAsync<T>(string resName, UnityAction<T> callBack) where T : Object
        {
            //�Զ���洢����
            string cacheName = $"{resName}_{typeof(T).Name}";

            ResourcesInfo<T> info;
            if (_nameToResInfoMap.ContainsKey(cacheName))
            {
                info = _nameToResInfoMap[cacheName] as ResourcesInfo<T>;
                //�������ü���
                ++info.RefCount;
                //�����첽������Դ
                if (info.Asset == null)
                    info.ResCallBack += callBack;
                else
                    callBack?.Invoke(info.Asset);
                return;
            }

            info = new ResourcesInfo<T>(callBack);
            _nameToResInfoMap.Add(cacheName, info);

            //ͨ��Mono����������Э��
            info.ResCoroutine = MonoManager.Instance.StartCoroutine(LoadAsync_Cor());

            IEnumerator LoadAsync_Cor()
            {
                //�첽������Դ
                ResourceRequest req = Resources.LoadAsync<T>(resName);
                yield return req;
                ResourcesInfo<T> info = _nameToResInfoMap[cacheName] as ResourcesInfo<T>;
                //�����ڴ�ɾ����ִ����Դ�ص�
                if (!info.IsDelete)
                {
                    //��¼��Դ
                    info.Asset = req.asset as T;
                    //���ûص�
                    info.Invoke();
                }
                //����Ͳ���¼��Դ��ж����Դ�����ֵ����Ƴ�
                else
                    UnloadAsset<T>(resName);
            }
        }

        /// <summary>
        /// ж��ָ����Դ
        /// </summary>
        /// <typeparam name="T">��Դ����</typeparam>
        /// <param name="resName">��Դ��</param>
        /// <param name="callBack">�Ƴ��Ļص�����, �ⲿ���ú��Դ˲���</param>
        public void UnloadAsset<T>(string resName) where T : Object
        {
            //�Զ���洢����
            string cacheName = $"{resName}_{typeof(T).Name}";
            ResourcesInfo<T> info;

            //�ֵ��д�������Դ��˵����Դ�����첽���ػ�������
            if (_nameToResInfoMap.ContainsKey(cacheName))
            {
                info = _nameToResInfoMap[cacheName] as ResourcesInfo<T>;
                if(!info.IsDelete)
                    //���Ǵ�ɾ����Դ���ż������ü���
                    --info.RefCount;
                //���ü���Ϊ0��������Դ��Ϊ��ɾ����Դ
                if(info.RefCount == 0 && !info.IsDelete)
                    info.IsDelete = true;
                //��Դ�������
                if (info.Asset != null && info.IsDelete)
                {
                    if (info.Asset is not GameObject)
                        //ж����Դ
                        Resources.UnloadAsset(info.Asset);

                    //�����ÿ�
                    info.Asset = null;
                    //���ֵ����Ƴ�
                    _nameToResInfoMap.Remove(cacheName);
                }
                //�������Դ�����첽���أ����������ﴦ��
            }
        }

        /// <summary>
        /// ж������δʹ�õ���Դ
        /// </summary>
        /// <param name="callBack">ж����ɻص�</param>
        public void UnloadUnusedAssets(UnityAction callBack = null)
        {
            MonoManager.Instance.StartCoroutine(UnLoadUnusedAssets_Cor(callBack));

            static IEnumerator UnLoadUnusedAssets_Cor(UnityAction callBack = null)
            {
                AsyncOperation ao = Resources.UnloadUnusedAssets();
                yield return ao;
                callBack?.Invoke();
            }
        }

        /// <summary>
        /// ���������Դ
        /// </summary>
        public void Clear()
        {
            _nameToResInfoMap.Clear();
            UnloadUnusedAssets();
            System.GC.Collect();
        }
    }
}
