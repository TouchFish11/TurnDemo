using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 任务源构建器
    /// ——统一创建任务完成源对象
    /// </summary>
    public class TaskSourceBuilder
    {
        /// <summary>
        /// 创建任务完成源
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TaskCompletionSource<TResult> CreateTCS<TResult>()
        {
            return new TaskCompletionSource<TResult>();
        }
    }
}
