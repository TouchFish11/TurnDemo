using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Mono
{
    /// <summary>
    /// Mono适配器接口
    /// 用于统一管理Unity生命周期回调
    /// </summary>
    public interface IMonoAdapter
    {
        /// <summary>
        /// 添加FixedUpdate帧事件监听
        /// </summary>
        /// <param name="fixedUpdateFun">FixedUpdate回调方法</param>
        void AddFixedUpdateListener(Action fixedUpdateFun);

        /// <summary>
        /// 添加LateUpdate帧事件监听
        /// </summary>
        /// <param name="lateUpdateFun">LateUpdate回调方法</param>
        void AddLateUpdateListener(Action lateUpdateFun);

        /// <summary>
        /// 添加Update帧事件监听
        /// </summary>
        /// <param name="updateFun">Update回调方法</param>
        void AddUpdateListener(Action updateFun);

        /// <summary>
        /// 移除FixedUpdate帧事件监听
        /// </summary>
        /// <param name="fixedUpdateFun">要移除的FixedUpdate回调方法</param>
        void RemoveFixedUpdateListener(Action fixedUpdateFun);

        /// <summary>
        /// 移除LateUpdate帧事件监听
        /// </summary>
        /// <param name="lateUpdateFun">要移除的LateUpdate回调方法</param>
        void RemoveLateUpdateListener(Action lateUpdateFun);

        /// <summary>
        /// 移除Update帧事件监听
        /// </summary>
        /// <param name="updateFun">要移除的Update回调方法</param>
        void RemoveUpdateListener(Action updateFun);

        /// <summary>
        /// 启动协程
        /// </summary>
        /// <param name="coroutine">协程迭代器方法</param>
        /// <returns>启动的协程对象，可用于后续停止协程</returns>
        Coroutine StartCoroutine(IEnumerator coroutine);
        
        /// <summary>
        /// 停止指定协程
        /// </summary>
        /// <param name="coroutine">需要停止的协程对象</param>
        void StopCoroutine(Coroutine coroutine);

        /// <summary>
        /// 应用程序退出事件
        /// 事件委托返回Task，支持异步退出逻辑处理
        /// </summary>
        event Func<Task> OnAppQuit;

        /// <summary>
        /// 应用程序暂停事件
        /// </summary>
        /// <returns>Task支持异步处理暂停逻辑</returns>
        event Func<bool, Task> OnAppPause;

        /// <summary>
        /// 应用程序焦点事件
        /// 应用程序获取/失去焦点时
        /// </summary>
        /// <returns>Task支持异步处理焦点逻辑</returns>
        event Func<bool, Task> OnAppFocus;
    }
}