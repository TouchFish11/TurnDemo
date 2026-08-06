using System.Collections;
using System.Collections.Generic;
using Core.DI;
using Core.Log;
using Core.Mono;
using Core.Time;
using UnityEngine;
using Logger = Core.Log.Logger;
using Object = UnityEngine.Object;

namespace Core.Pool
{
    /// <summary>
    /// 继承Mono的对象池管理类
    /// 用于管理游戏对象的复用，减少频繁创建/销毁对象的性能开销
    /// 对象池存储主逻辑组件（如 Bullet），主组件内部缓存其他依赖组件（Rigidbody、Collider 等），外部只与主组件交互。
    /// 如果某个对象没有明确的主逻辑组件（例如一个纯视觉特效，只有 ParticleSystem 和 Light），此时可以直接用 GameObject 作为池存储类型，或者选择最常被访问的那个组件作为主组件（如 ParticleSystem）。
    /// </summary>
    internal class ObjectPool<T> : IPool<T> where T : Object
    {
        [Inject] private IMonoAdapter _monoAdapter;
        // 存储未使用对象
        private readonly Stack<T> _unUsedMonos = new();
        // 对象池的父物体（用于统一管理池内对象的层级）
        private GameObject _parentObj;
        // 是否开启布局管理
        private readonly bool _isOpenLayout;
        // 活跃时间阈值，大于等于该数值活跃，小于则惰性
        private readonly float _activeTimeThreshold;
        // 最小缓存数量
        private readonly int _minSize;
        // 最大缓存容量
        private readonly int _maxSize;
        // 每帧销毁数
        private const int _destroyCountPerFrame = 30;

        public int ActiveCount { get; private set; }

        public string PoolId { get; }

        public bool IsLazy => _activeTimeThreshold > TimeUtil.RealtimeSinceStartup - LastUsedTime;
        
        public float LastUsedTime { get; private set; }
        
        public int InactiveCount => _unUsedMonos.Count;

        /// <summary>
        /// 构造函数：初始化对象池
        /// </summary>
        /// <param name="rootObj">对象池根节点（所有池对象的顶级父物体）</param>
        /// <param name="poolObjName">当前对象池的名称（用于命名父物体）</param>
        /// <param name="isOpenLayout">是否采用对象池布局</param>
        /// <param name="activeTimeThreshold">活跃时间阈值</param>
        /// <param name="minSize">最小缓存数量</param>
        /// <param name="maxSize">最大缓存容量</param>
        public ObjectPool(GameObject rootObj, string poolObjName, bool isOpenLayout, float activeTimeThreshold, int minSize, int maxSize)
        {
            // 若父物体未创建且开启布局管理，创建池的父物体并设置层级
            if (isOpenLayout)
            {
                _parentObj = new GameObject(poolObjName);
                _parentObj.transform.SetParent(rootObj.transform, false);
            }

            ActiveCount = 1;
            PoolId = poolObjName;
            _isOpenLayout = isOpenLayout;
            _activeTimeThreshold = activeTimeThreshold;
            _minSize = minSize;
            _maxSize = maxSize;
            LastUsedTime = TimeUtil.RealtimeSinceStartup;
        }
        
        public T Get()
        {
            // 检查栈中是否有未使用的对象
            if (_unUsedMonos.Count <= 0) 
                return null;
            
            // 弹出栈顶的未使用对象
            var obj = _unUsedMonos.Pop();
            switch (obj)
            {
                // 激活对象使其可见/可用
                case GameObject gameObject:
                    gameObject.SetActive(true);
                    // 解除对象与池父物体的父子关系（让对象归回业务逻辑层级）
                    gameObject.transform.SetParent(null, false);
                    break;
                case Component component:
                    component.gameObject.SetActive(true);
                    // 解除对象与池父物体的父子关系（让对象归回业务逻辑层级）
                    component.gameObject.transform.SetParent(null, false);
                    break;
            }

            // 该类型池增加使用次数
            ++ActiveCount;
            // 记录使用时间
            LastUsedTime = TimeUtil.RealtimeSinceStartup;
            return obj;
        }

        public void Push(T obj)
        {
            // 超出上限直接销毁
            if (InactiveCount >= _maxSize)
            {
                ReleaseInternal(obj);
                Logger.LogWarning(ELogTags.Pool, $"Pool '{PoolId}' is full(MaxCapacity: {_maxSize}), make obj destroyed");
                return;
            }
            
            switch (obj)
            {
                case GameObject gameObject:
                    // 若开启布局管理，将对象归位到池的父物体下统一管理
                    if (_isOpenLayout)
                    {
                        gameObject.transform.SetParent(_parentObj.transform, false);
                    }
                    // 禁用对象使其不可见/不可用
                    gameObject.SetActive(false);
                    break;
                case Component component:
                    if (_isOpenLayout)
                    {
                        component.gameObject.transform.SetParent(_parentObj.transform, false);
                    }
                    // 禁用对象使其不可见/不可用
                    component.gameObject.SetActive(false);
                    break;
            }

            // 将对象压入未使用栈，等待下次复用
            _unUsedMonos.Push(obj);
            // 该类型池减少使用次数
            --ActiveCount;
        }
        
        public void Trim()
        {
            var count = _unUsedMonos.Count;
            if(count <= _minSize)
                return;

            _monoAdapter.StartCoroutine(Trim_Cor());
        }

        /// <summary>
        /// 分帧释放，避免卡顿
        /// </summary>
        /// <returns></returns>
        private IEnumerator Trim_Cor()
        {
            var releaseCountPerFrame = 0;
            while (_unUsedMonos.Count > _minSize)
            {
                ReleaseInternal(_unUsedMonos.Pop());
                ++releaseCountPerFrame;
                if (releaseCountPerFrame >= _destroyCountPerFrame - 1)
                    yield return null;
            }
        }
        
        /// <summary>
        /// 清空对象池销毁所有对象
        /// </summary>
        public void ClearAll()
        {
            while (_unUsedMonos.Count > 0)
            {
                ReleaseInternal(_unUsedMonos.Pop());
            }
            
            // 清空未使用对象栈
            _unUsedMonos.Clear();
            // 销毁池的父物体（连带销毁所有子物体）
            if(_parentObj)
                EngineUtility.Destroy(_parentObj);
            // 置空父物体引用，防止空引用异常
            _parentObj = null;
        }

        private static void ReleaseInternal(T releaseObj)
        {
            switch (releaseObj)
            {
                case GameObject gameObject:
                    EngineUtility.Destroy(gameObject);
                    break;
                case Component component:
                    EngineUtility.Destroy(component.gameObject);
                    break;
            }
        }
    }
}