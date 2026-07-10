using System;
using System.Collections.Generic;
using Core.Mono;
using Core.Utility;
using UnityEngine;
using Logger = Core.Log.Logger;
using Object = UnityEngine.Object;

namespace Core.Pool
{
    /// <summary>
    /// 继承Mono的对象池管理类
    /// 用于管理游戏对象的复用，减少频繁创建/销毁对象的性能开销
    /// 注意：不要问“一个对象多个组件怎么存池”，而要问“为什么这些组件没有统一收口到一个主组件里”。遵循单一主组件模式，对象池直接存储该主组件类型
    /// 对象池存储主逻辑组件（如 Bullet），主组件内部缓存其他依赖组件（Rigidbody、Collider 等），外部只与主组件交互。
    /// 如果某个对象没有明确的主逻辑组件（例如一个纯视觉特效，只有 ParticleSystem 和 Light），此时可以直接用 GameObject 作为池存储类型，或者选择最常被访问的那个组件作为主组件（如 ParticleSystem）。但在实际项目中，这类对象通常也会挂一个 PooledEffect 脚本来统一管理重置逻辑。
    /// </summary>
    internal class ObjectPool
    {
        // 存储未使用对象的栈结构（栈结构适合后进先出的复用逻辑）
        private readonly Stack<Object> _unUsedObjStack = new();
        // 对象池的父物体（用于统一管理池内对象的层级）
        private GameObject _parentObj;
        // 类型标识
        private readonly Type _objectType;
        // 是否开启布局管理
        private readonly bool _isOpenLayout;
        
        /// <summary>
        /// 池ID，以对象名称为准
        /// </summary>
        public string PoolId { get; private set; }
        
        /// <summary>
        /// 使用次数，当前正在使用的对象数量
        /// </summary>
        public uint UsedCount { get; private set; }
        
        /// <summary>
        /// 上次使用时间，越小则越早使用
        /// </summary>
        public double LastUsedTime { get; private set; }
        
        /// <summary>
        /// 池化对象类型
        /// </summary>
        public EObjectType ObjectType { get; private set; }

        /// <summary>
        /// 构造函数：初始化对象池
        /// </summary>
        /// <param name="rootObj">对象池根节点（所有池对象的顶级父物体）</param>
        /// <param name="poolObjName">当前对象池的名称（用于命名父物体）</param>
        /// <param name="type">池子对象中存储的对象的实际类型</param>
        /// <param name="isOpenLayout">是否采用对象池布局</param>
        /// <param name="objectType">当前池子的缓存的对象类别</param>
        public ObjectPool(GameObject rootObj, string poolObjName, Type type, bool isOpenLayout, EObjectType objectType)
        {
            // 若父物体未创建且开启布局管理，创建池的父物体并设置层级
            if (isOpenLayout)
            {
                _parentObj = new GameObject(poolObjName);
                _parentObj.transform.SetParent(rootObj.transform, false);
            }

            PoolId = poolObjName;
            _objectType = type;
            _isOpenLayout = isOpenLayout;
            UsedCount = 0;
            LastUsedTime = TimeUtil.RealtimeSinceStartupAsDouble;
            ObjectType = objectType;
        }

        /// <summary>
        /// 从对象池中获取未使用的对象
        /// </summary>
        /// <returns>可用的游戏对象（无可用对象时返回null）</returns>
        public T Get<T>() where T : Object
        {
            if (typeof(T) != _objectType && !typeof(T).IsAssignableFrom(_objectType))
            {
                Logger.LogError(TODO, $"[{nameof(ObjectPool)}]: Type mismatch: requested {typeof(T)}, pool stores {_objectType}");
                return null;
            }
            
            // 检查栈中是否有未使用的对象
            if (_unUsedObjStack.Count <= 0) 
                return null;
            
            // 弹出栈顶的未使用对象
            var obj = _unUsedObjStack.Pop();
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
            ++UsedCount;
            // 记录使用时间
            LastUsedTime = TimeUtil.RealtimeSinceStartupAsDouble;
            return (T)obj;
        }

        /// <summary>
        /// 将对象回收至对象池
        /// </summary>
        /// <param name="obj">需要回收的游戏对象</param>
        public void Push(Object obj)
        {
            // 若开启布局管理，将对象归位到池的父物体下统一管理
            if (_isOpenLayout)
            {
                switch (obj)
                {
                    case GameObject gameObject:
                        gameObject.transform.SetParent(_parentObj.transform, false);
                        // 禁用对象使其不可见/不可用
                        gameObject.SetActive(false);
                        break;
                    case Component component:
                        component.transform.SetParent(_parentObj.transform, false);
                        // 禁用对象使其不可见/不可用
                        component.gameObject.SetActive(false);
                        break;
                }
            }

            // 将对象压入未使用栈，等待下次复用
            _unUsedObjStack.Push(obj);
            // 该类型池减少使用次数
            --UsedCount;
        }

        /// <summary>
        /// 清空对象池销毁所有对象
        /// </summary>
        public void Clear()
        {
            while (_unUsedObjStack.Count > 0)
            {
                var obj = _unUsedObjStack.Pop();
                switch (obj)
                {
                    case GameObject gameObject:
                        EngineUtility.Destroy(gameObject);
                        break;
                    case Component component:
                        EngineUtility.Destroy(component.gameObject);
                        break;
                }
            }
            
            // 清空未使用对象栈
            _unUsedObjStack.Clear();
            // 销毁池的父物体（连带销毁所有子物体）
            if(_parentObj)
                EngineUtility.Destroy(_parentObj);
            // 置空父物体引用，防止空引用异常
            _parentObj = null;
        }

        /// <summary>
        /// 获取未使用对象的数量
        /// </summary>
        public int UnUsedCount => _unUsedObjStack.Count;
    }
}