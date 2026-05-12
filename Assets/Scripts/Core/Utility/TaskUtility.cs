using System;
using System.Collections;
using System.Threading.Tasks;
using Core.Mono;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace Core.Utility
{
    /// <summary>
    /// 任务工具类
    /// </summary>
    public static class TaskUtility
    {
        /// <summary>
        /// 等待条件为true时任务完成
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">当condition为null时抛出</exception>
        public static async Task WaitUntil(Func<bool> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            while (!condition())
            {
                await Task.Yield();
            }
        }

        /// <summary>
        /// 等待任务完成
        /// Task转换为协程；task的IsFaulted为true时，表示任务执行失败
        /// </summary>
        /// <param name="task">要转换的任务</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">task为null时抛出</exception>
        public static IEnumerator WaitForTask(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsFaulted)
            {
                Logger.LogError($"{nameof(TaskUtility)}.{nameof(WaitForTask)}: {task.Exception}，StackTrance：{task.Exception?.StackTrace}");
            }
        }
        
        /// <summary>
        /// 等待任务完成
        /// Task转换为协程；task的IsFaulted为true时，表示任务执行失败；否则执行callback
        /// </summary>
        /// <param name="task"></param>
        /// <param name="callback"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerator WaitForTask<T>(Task<T> task, Action<T> callback)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsFaulted)
            {
                Logger.LogError($"{nameof(TaskUtility)}.{nameof(WaitForTask)}: {task.Exception}，StackTrance：{task.Exception?.StackTrace}");
            }
            else
            {
                callback?.Invoke(task.Result);
            }
        }

        /// <summary>
        /// 等待协程完成
        /// </summary>
        /// <param name="coroutine"></param>
        /// <param name="monoAdapter"></param>
        /// <returns></returns>
        public static Task WaitForCoroutine(IEnumerator coroutine, IMonoAdapter monoAdapter)
        {
            var tcs = new TaskCompletionSource<bool>();
            monoAdapter.StartCoroutine(RunCoroutine());
            return tcs.Task;
            
            IEnumerator RunCoroutine()
            {
                yield return coroutine;
                tcs.SetResult(true);
            }
        }
    }
}
