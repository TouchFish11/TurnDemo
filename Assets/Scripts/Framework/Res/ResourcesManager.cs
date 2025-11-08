using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// Resources资源管理器
    /// </summary>
    public class ResourcesManager : SingletonBase<ResourcesManager>
    {
        //存储资源的字典
        private readonly Dictionary<string, BaseResourcesInfo> _resDic = new Dictionary<string, BaseResourcesInfo>();

        private ResourcesManager() { }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="resPath">资源路径</param>
        /// <returns></returns>
        public T Load<T>(string resPath) where T : Object
        {
            //自定义存储名称
            string cacheName = $"{resPath}_{typeof(T).Name}";
            ResourcesInfo<T> info = null;
            if (_resDic.ContainsKey(cacheName))
            {
                info = _resDic[cacheName] as ResourcesInfo<T>;
                if (info.Asset == null)
                {
                    MonoManager.Instance.StopCoroutine(info.ResCoroutine);
                    //置空协程
                    info.ResCoroutine = null;
                    //同步加载，记录资源
                    info.Asset = Resources.Load<T>(resPath);
                    //执行回调
                    info.Invoke();
                    return info.Asset;
                }
                else
                {
                    return info.Asset;
                }
            }

            info = new ResourcesInfo<T>(null);
            //存储到字典中
            _resDic.Add(cacheName, info);
            //同步加载，记录资源
            info.Asset = Resources.Load<T>(resPath);
            return info.Asset;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="resName">资源路径</param>
        /// <param name="callBack">回调函数</param>
        public void LoadAsync<T>(string resName, UnityAction<T> callBack) where T : Object
        {
            //自定义存储名称
            string cacheName = $"{resName}_{typeof(T).Name}";

            ResourcesInfo<T> info;
            if (_resDic.ContainsKey(cacheName))
            {
                info = _resDic[cacheName] as ResourcesInfo<T>;
                //增加引用计数
                ++info.RefCount;
                //正在异步加载资源
                if (info.Asset == null)
                    info.ResCallBack += callBack;
                else
                    callBack?.Invoke(info.Asset);
                return;
            }

            info = new ResourcesInfo<T>(callBack);
            _resDic.Add(cacheName, info);

            //通过Mono管理器开启协程
            info.ResCoroutine = MonoManager.Instance.StartCoroutine(LoadAsync_Cor());

            IEnumerator LoadAsync_Cor()
            {
                //异步加载资源
                ResourceRequest req = Resources.LoadAsync<T>(resName);
                yield return req;
                ResourcesInfo<T> info = _resDic[cacheName] as ResourcesInfo<T>;
                //不处于待删除才执行资源回调
                if (!info.IsDelete)
                {
                    //记录资源
                    info.Asset = req.asset as T;
                    //调用回调
                    info.Invoke();
                }
                //否则就不记录资源，卸载资源，从字典中移除
                else
                    UnloadAsset<T>(resName);
            }
        }

        /// <summary>
        /// 卸载指定资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="resName">资源名</param>
        /// <param name="callBack">移除的回调函数, 外部调用忽略此参数</param>
        public void UnloadAsset<T>(string resName) where T : Object
        {
            //自定义存储名称
            string cacheName = $"{resName}_{typeof(T).Name}";
            ResourcesInfo<T> info;

            //字典中存在在资源，说明资源正在异步加载或加载完毕
            if (_resDic.ContainsKey(cacheName))
            {
                info = _resDic[cacheName] as ResourcesInfo<T>;
                if(!info.IsDelete)
                    //不是待删除资源，才减少引用计数
                    --info.RefCount;
                //引用计数为0，将该资源变为待删除资源
                if(info.RefCount == 0 && !info.IsDelete)
                    info.IsDelete = true;
                //资源加载完毕
                if (info.Asset != null && info.IsDelete)
                {
                    if (info.Asset is not GameObject)
                        //卸载资源
                        Resources.UnloadAsset(info.Asset);

                    //引用置空
                    info.Asset = null;
                    //从字典中移除
                    _resDic.Remove(cacheName);
                }
                //否则该资源正在异步加载，不用在这里处理
            }
        }

        /// <summary>
        /// 卸载所有未使用的资源
        /// </summary>
        /// <param name="callBack">卸载完成回调</param>
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
        /// 清空所有资源
        /// </summary>
        public void Clear()
        {
            _resDic.Clear();
            UnloadUnusedAssets();
            System.GC.Collect();
        }
    }
}
