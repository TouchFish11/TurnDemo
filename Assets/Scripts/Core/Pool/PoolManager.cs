using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Global;
using Core.Singleton;
using UnityEngine;

namespace Core.Pool
{
    /// <summary>
    /// 缓存池管理器
    /// </summary>
    public class PoolManager : SingletonBase<PoolManager>, IPoolManager
    {
        public override int InitPriority => 0;
        // 存储继承Mono对象
        private readonly Dictionary<string, PoolObj> _poolObjDic = new();
        // 存储不继承Mono对象
        private readonly Dictionary<string, BasePoolData> _poolDataDic = new();
        // 缓存池根对象
        private GameObject _poolRootObj;
        private int priority;

        private PoolManager(){}

        public override Task InitAsync()
        {
            return Task.CompletedTask;
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
        
        public GameObject GetAssetBundleObj(string abName, string assetName)
        {
            // 存在该对象就取出来使用
            if (_poolObjDic.ContainsKey(assetName) && _poolObjDic[assetName].UnUsedCount > 0)
            {
                return _poolObjDic[assetName].Get();
            }

            return null;
        }

        public void PushObj(GameObject obj)
        {
            // 第一次缓存对象时初始化缓存池存储结构
            if (!_poolRootObj && GlobalSettings.Instance.isOpenLayout)
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

        /// <summary>
        /// 获取指定资源缓存的数量
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public int GetUnUsedCount(string assetName)
        {
            return _poolObjDic.TryGetValue(assetName, out var obj) ? obj.UnUsedCount : 0;
        }
        
        public int ClearCache(string assetName)
        {
            if (!_poolObjDic.TryGetValue(assetName, out var poolObj))
            {
                return 0;
            }

            var count = poolObj.UnUsedCount;
            poolObj.Clear();
            _poolObjDic.Remove(assetName);
            return count;
        }
        
        public void ClearAll()
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
