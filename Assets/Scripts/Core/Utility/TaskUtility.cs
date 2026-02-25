using System;
using System.Collections;
using System.Threading.Tasks;
using Core.Log;

namespace Core.Utility
{
    /// <summary>
    /// ���񹤾���
    /// </summary>
    public static class TaskUtility
    {
        /// <summary>
        /// �첽WaitUntil
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
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
        /// Task转换为协程
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static IEnumerator WaitForTask(Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsFaulted)
            {
                LogManager.LogError($"{nameof(TaskUtility)}.{nameof(WaitForTask)}: {task.Exception}");
            }
        }
        
        /// <summary>
        /// 等待任务完成
        /// Task转换为协程
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
                LogManager.LogError($"{nameof(TaskUtility)}.{nameof(WaitForTask)}: {task.Exception}");
            }
            else
            {
                callback?.Invoke(task.Result);
            }
        }
    }
}
