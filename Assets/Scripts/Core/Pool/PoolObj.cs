using System.Collections.Generic;
using Core.Global;
using UnityEngine;

namespace Core.Pool
{
    /// <summary>
    /// 继承Mono的对象池管理类
    /// 用于管理游戏对象的复用，减少频繁创建/销毁对象的性能开销
    /// </summary>
    public class PoolObj
    {
        // 存储未使用对象的栈结构（栈结构适合后进先出的复用逻辑）
        private readonly Stack<GameObject> _unUsedObjStack = new();
        // 对象池的父物体（用于统一管理池内对象的层级）
        private GameObject _parentObj;

        /// <summary>
        /// 构造函数：初始化对象池
        /// </summary>
        /// <param name="rootObj">对象池根节点（所有池对象的顶级父物体）</param>
        /// <param name="poolObjName">当前对象池的名称（用于命名父物体）</param>
        public PoolObj(GameObject rootObj, string poolObjName)
        {
            // 若父物体未创建且开启布局管理，创建池的父物体并设置层级
            if (GlobalSettings.Instance.isOpenLayout)
            {
                _parentObj = new GameObject(poolObjName);
                _parentObj.transform.SetParent(rootObj.transform, false);
            }
        }

        /// <summary>
        /// 从对象池中获取未使用的对象
        /// </summary>
        /// <returns>可用的游戏对象（无可用对象时返回null）</returns>
        public GameObject Get()
        {
            GameObject obj = null;
            // 检查栈中是否有未使用的对象
            if (_unUsedObjStack.Count > 0)
            {
                // 弹出栈顶的未使用对象
                obj = _unUsedObjStack.Pop();
                // 激活对象使其可见/可用
                obj.SetActive(true);
                // 解除对象与池父物体的父子关系（让对象归回业务逻辑层级）
                obj.transform.SetParent(null, false);
            }
            return obj;
        }

        /// <summary>
        /// 将对象回收至对象池
        /// </summary>
        /// <param name="obj">需要回收的游戏对象</param>
        public void Push(GameObject obj)
        {
            // 若开启布局管理，将对象归位到池的父物体下统一管理
            if (GlobalSettings.Instance.isOpenLayout)
            {
                obj.transform.SetParent(_parentObj.transform, false);
            }
            // 禁用对象使其不可见/不可用
            obj.SetActive(false);
            // 将对象压入未使用栈，等待下次复用
            _unUsedObjStack.Push(obj);
        }

        /// <summary>
        /// 清空对象池释放所有资源
        /// </summary>
        public void Clear()
        {
            while (_unUsedObjStack.Count > 0)
            {
                Object.Destroy(_unUsedObjStack.Pop());
            }
            // 清空未使用对象栈
            _unUsedObjStack.Clear();
            // 销毁池的父物体（连带销毁所有子物体）
            Object.Destroy(_parentObj);
            // 置空父物体引用，防止空引用异常
            _parentObj = null;
        }

        /// <summary>
        /// 只读属性：获取未使用对象的数量
        /// </summary>
        public int UnUsedCount => _unUsedObjStack.Count;
    }
}