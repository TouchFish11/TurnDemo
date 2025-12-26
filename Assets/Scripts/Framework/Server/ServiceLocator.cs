using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 全局服务定位器
    /// </summary>
    public class ServiceLocator : SingletonAutoMono<ServiceLocator>
    {
        // 服务类型到服务的映射
        private readonly Dictionary<Type, object> _typeToServerMap = new Dictionary<Type, object>();

        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service"></param>
        public void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (_typeToServerMap.ContainsKey(type))
            {
                LogManager.Log($"{type.Name}已存在，覆盖旧实例");
            }
            _typeToServerMap[type] = service;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : class
        {
            var type = typeof(T);
            if (_typeToServerMap.TryGetValue(type, out var service))
            {
                return service as T;
            }
            Debug.LogError($"未找到{type.Name}");
            return null;
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Unregister<T>() where T : class
        {
            var type = typeof(T);
            if (_typeToServerMap.ContainsKey(type))
            {
                _typeToServerMap.Remove(type);
            }
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void Clear()
        {
            _typeToServerMap.Clear();
        }
    }
}
