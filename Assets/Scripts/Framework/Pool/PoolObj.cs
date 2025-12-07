using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 继承Mono的缓存对象
    /// </summary>
    public class PoolObj
    {
        //存储未使用对象的栈
        private readonly Stack<GameObject> _unUsedObjStack = new Stack<GameObject>();
        //该类对象的父对象
        private readonly GameObject _parentObj;

        public PoolObj(GameObject rootObj, string poolObjName)
        {
            if (_parentObj == null && GlobalSettings.Instance.IsOpenLayout)
            {
                _parentObj = new GameObject(poolObjName);
                _parentObj.transform.SetParent(rootObj.transform, false);
            }
        }

        /// <summary>
        /// 获取未使用的对象
        /// </summary>
        /// <returns>缓存的对象</returns>
        public GameObject Get()
        {
            GameObject obj = null;
            if (_unUsedObjStack.Count > 0)
            {
                //获取未使用的对象
                obj = _unUsedObjStack.Pop();
                //激活对象
                obj.SetActive(true);
                //断开父子关系
                obj.transform.SetParent(null, false);
            }
            return obj;
        }

        /// <summary>
        /// 缓存对象
        /// </summary>
        /// <param name="obj">不使用的对象</param>
        public void Push(GameObject obj)
        {
            if (GlobalSettings.Instance.IsOpenLayout)
            {
                //设置父对象
                obj.transform.SetParent(_parentObj.transform, false);
            }
            //失活对象
            obj.SetActive(false);
            //存储进容器
            _unUsedObjStack.Push(obj);
        }

        /// <summary>
        /// 未使用对象数量
        /// </summary>
        public int UnUsedCount => _unUsedObjStack.Count;
    }
}
