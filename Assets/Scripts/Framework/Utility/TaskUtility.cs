using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 任务工具类
/// </summary>
public class TaskUtility
{
    /// <summary>
    /// 异步WaitUntil
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
            // 等待一帧结束，等同于协程的 yield return null
            await Task.Yield();
        }
    }

    /// <summary>
    /// 等待任务完成
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
            Debug.LogError($"任务执行错误: {task.Exception}");
        }
    }
}
