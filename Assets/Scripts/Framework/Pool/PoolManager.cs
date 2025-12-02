using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 缓存池管理器
    /// </summary>
    public class PoolManager : SingletonBase<PoolManager>
    {
        //存储继承Mono对象
        private readonly Dictionary<string, PoolObj> _poolObjDic = new Dictionary<string, PoolObj>();

        //存储不继承Mono对象
        private readonly Dictionary<string, BasePoolData> _poolDataDic = new Dictionary<string, BasePoolData>();

        //缓存池根对象
        private GameObject _poolRootObj;

        private PoolManager()
        {

        }

        /// <summary>
        /// 获取非AB包中的缓存对象
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="assetName">资源名称</param>
        /// <returns></returns>
        public T GetObj<T>(string assetName) where T : Behaviour
        {
            // 存在该对象就取出来使用
            if (_poolObjDic.ContainsKey(assetName) && _poolObjDic[assetName].UnUsedCount > 0)
            {
                return _poolObjDic[assetName].Get().GetComponent<T>();
            }

            GameObject newObj = new GameObject(assetName);
            return newObj.AddComponent<T>();
        }

        /// <summary>
        /// 异步获取来自AB包的缓存对象
        /// </summary>
        /// <param name="assetBundleType">AB包类型</param>
        /// <param name="assetName">资源名称</param>
        /// <param name="objCallBack">对象加载完成回调</param>
        /// <returns></returns>
        public async Task<GameObject> GetAssetBundleObjAsync(E_AssetBundleType assetBundleType, string assetName)
        {
            // 存在该对象就取出来使用
            if (_poolObjDic.ContainsKey(assetName) && _poolObjDic[assetName].UnUsedCount > 0)
            {
                return _poolObjDic[assetName].Get();
            }

#if EDITOR_TEST_AB || !UNITY_EDITOR
            // AB包异步加载
            GameObject obj = await AssetBundleManager.Instance.LoadAssetAsync<GameObject>(assetBundleType, assetName);
            // 实例化预设体
            GameObject instanceObj = GameObject.Instantiate(obj);
            // 避免实例化出的对象的名字后带有(Clone)
            instanceObj.name = assetName;
            return instanceObj;
#else
            // 加载编辑器路径下的资源
            GameObject obj = EditorResMgr.Instance.LoadEditorAsset<GameObject>(assetName);
            // 实例化预设体
            GameObject instanceObj = GameObject.Instantiate(obj);
            // 避免实例化出的对象的名字后带有(Clone)
            instanceObj.name = assetName;
            return instanceObj;
#endif
        }

        /// <summary>
        /// 缓存继承Mono的对象
        /// </summary>
        /// <param name="obj">游戏对象</param>
        public void PushObj(GameObject obj)
        {
            //第一次缓存对象时初始化缓存池存储结构
            if (_poolRootObj == null && GlobalSettings.Instance.IsOpenLayout)
                _poolRootObj = new GameObject("Pool");

            //已经存储过了就可以直接往容器中存储对象
            if (_poolObjDic.ContainsKey(obj.name))
                _poolObjDic[obj.name].Push(obj);
            else
            {
                //第一次存储要创建存储容器
                PoolObj poolObj = new PoolObj(_poolRootObj, obj.name);
                poolObj.Push(obj);
                _poolObjDic.Add(obj.name, poolObj);
            }
        }

        /// <summary>
        /// 获取未继承Mono的对象
        /// </summary>
        /// <typeparam name="T">类名</typeparam>
        /// <param name="nameSpace">可选参数：命名空间</param>
        /// <returns></returns>
        public T GetData<T>(string nameSpace = "") where T : class, IPoolData, new()
        {
            //自定义获取名称，与存储名称一致
            string dataName = nameSpace + "_" + typeof(T).Name;
            if (_poolDataDic.ContainsKey(dataName))
            {
                PoolData<T> poolData = _poolDataDic[dataName] as PoolData<T>;
                if (poolData.UnUsedCount > 0)
                    return poolData.Get();
            }
            return new T();
        }

        /// <summary>
        /// 缓存未继承Mono的对象
        /// </summary>
        /// <typeparam name="T">类名</typeparam>
        /// <param name="data">数据对象</param>
        /// <param name="nameSpace">可选参数：命名空间</param>
        public void PushData<T>(T data, string nameSpace = "") where T : class, IPoolData, new()
        {
            //自定义缓存名称，与获取名称一致
            string dataName = nameSpace + "_" + typeof(T).Name;
            if( _poolDataDic.ContainsKey(dataName))
                (_poolDataDic[dataName] as PoolData<T>).Push(data);
            else
            {
                PoolData<T> poolData = new PoolData<T>();
                poolData.Push(data);
                _poolDataDic.Add(dataName, poolData);
            }
        }

        /// <summary>
        /// 清空缓存池
        /// </summary>
        public void Clear()
        {
            _poolRootObj = null;
            _poolObjDic.Clear();
            _poolDataDic.Clear();
            GC.Collect();
        }
    }
}
