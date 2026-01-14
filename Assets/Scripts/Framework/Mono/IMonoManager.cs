using System;
using System.Collections;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Mono管理器接口
    /// </summary>
    public interface IMonoManager
    {
        /// <summary>
        /// 添加物理帧更新监听函数
        /// </summary>
        /// <param name="fixedUpdateFun">物理帧更新监听函数</param>
        void AddFixedUpdateListener(Action fixedUpdateFun);

        /// <summary>
        /// 添加后期帧更新监听函数
        /// </summary>
        /// <param name="lateUpdateFun">后期帧更新监听函数</param>
        void AddLateUpdateListener(Action lateUpdateFun);

        /// <summary>
        /// 添加帧更新监听函数
        /// </summary>
        /// <param name="updateFun">帧更新监听函数</param>
        void AddUpdateListener(Action updateFun);

        /// <summary>
        /// 移除物理帧更新监听函数
        /// </summary>
        /// <param name="fixedUpdateFun">物理帧更新监听函数</param>
        void RemoveFixedUpdateListener(Action fixedUpdateFun);

        /// <summary>
        /// 移除后期帧更新监听函数
        /// </summary>
        /// <param name="lateUpdateFun">后期帧更新监听函数</param>
        void RemoveLateUpdateListener(Action lateUpdateFun);

        /// <summary>
        /// 移除帧更新监听函数
        /// </summary>
        /// <param name="updateFun">帧更新监听函数</param>

        void RemoveUpdateListener(Action updateFun);

        /// <summary>
        /// 开启协程
        /// </summary>
        /// <param name="coroutine"></param>
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}
