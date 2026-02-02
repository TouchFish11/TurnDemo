using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Global;
using Core.Service;
using Core.Singleton;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Pool
{
    /// <summary>
    /// 缓存池管理器
    /// </summary>
    public class PoolManager : SingletonBase<PoolManager>, IPoolManager
    {
        // 存储继承Mono对象
        private readonly Dictionary<string, PoolObj> _poolObjDic = new();
        // 存储不继承Mono对象
        private readonly Dictionary<string, BasePoolData> _poolDataDic = new();
        // 缓存池根对象
        private GameObject _poolRootObj;

        private PoolManager()
        {

        }

        public T GetObj<T>(string assetName) where T : Behaviour
        {
            // 存在该对象就取出来使用
            if (_poolObjDic.ContainsKey(assetName) && _poolObjDic[assetName].UnUsedCount > 0)
            {
                return _poolObjDic[assetName].Get().GetComponent<T>();
            }

            var newObj = new GameObject(assetName);
            return newObj.AddComponent<T>();
        }
        
        // TODO：优化为只获取，不创建；对象该方法进行再次封装调用，外部判空后再用ab包加载
        public async Task<GameObject> GetAssetBundleObjAsync(EAssetBundleType assetBundleType, string assetName)
        {
            // 存在该对象就取出来使用
            if (_poolObjDic.ContainsKey(assetName) && _poolObjDic[assetName].UnUsedCount > 0)
            {
                return _poolObjDic[assetName].Get();
            }

#if EDITOR_TEST_AB || !UNITY_EDITOR
            // AB包异步加载
            var obj = await ServiceLocator.Get<IAssetBundleManager>().LoadAssetAsync<GameObject>(assetBundleType, assetName);
            // 实例化预设体
            var instanceObj = Object.Instantiate(obj);
            // 避免实例化出的对象的名字后带有(Clone)
            instanceObj.name = assetName;
            return instanceObj;
#else
            await Task.CompletedTask;
            // 加载编辑器路径下的资源
            GameObject obj = EditorResManager.Instance.LoadEditorAsset<GameObject>(assetName);
            // 实例化预设体
            GameObject instanceObj = GameObject.Instantiate(obj);
            // 避免实例化出的对象的名字后带有(Clone)
            instanceObj.name = assetName;
            return instanceObj;
#endif
        }

        public void PushObj(GameObject obj)
        {
            // 第一次缓存对象时初始化缓存池存储结构
            if (_poolRootObj == null && GlobalSettings.Instance.isOpenLayout)
            {
                _poolRootObj = new GameObject("Pool");
            }

            // 已经存储过了就可以直接往容器中存储对象
            if (_poolObjDic.TryGetValue(obj.name, out var poolObj))
            {
                poolObj.Push(obj);
            }
            else
            {
                // 第一次存储要创建存储容器
                poolObj = new PoolObj(_poolRootObj, obj.name);
                poolObj.Push(obj);
                _poolObjDic.Add(obj.name, poolObj);
            }
        }
        
        public T GetData<T>(string nameSpace = "") where T : class, IPoolData, new()
        {
            // 自定义获取名称，与存储名称一致
            var dataName = $"{nameSpace}_{typeof(T).Name}";
            if (!_poolDataDic.TryGetValue(dataName, out var basePoolData))
            {
                return new T();
            }

            if (basePoolData is PoolData<T> poolData && poolData.UnUsedCount > 0)
            {
                return poolData.Get();
            }
            return new T();
        }

        public void PushData<T>(T data, string nameSpace = "") where T : class, IPoolData, new()
        {
            // 自定义缓存名称，与获取名称一致
            var dataName = $"{nameSpace}_{typeof(T).Name}";
            if (_poolDataDic.TryGetValue(dataName, out var basePoolData))
            {
                (basePoolData as PoolData<T>)?.Push(data);
            }
            else
            {
                var poolData = new PoolData<T>();
                poolData.Push(data);
                _poolDataDic.Add(dataName, poolData);
            }
        }
        
        public void ClearTypes(params Type[] types)
        {
            foreach (var type in types)
            {
                var typeName = type.Name;
                if (!_poolObjDic.TryGetValue(typeName, out var poolObj))
                {
                    continue;
                }
                
                poolObj.Clear();
                _poolObjDic.Remove(typeName);
            }
        }
        
        public void Clear()
        {
            foreach (var poolObj in _poolObjDic.Values)
            {
                poolObj.Clear();
            }

            _poolRootObj = null;
            _poolObjDic.Clear();
            _poolDataDic.Clear();
            GC.Collect();
        }
    }
}
