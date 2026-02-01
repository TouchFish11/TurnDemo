using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

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
                // �ȴ�һ֡��������ͬ��Э�̵� yield return null
                await Task.Yield();
            }
        }

        /// <summary>
        /// �ȴ��������
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
                Debug.LogError($"����ִ�д���: {task.Exception}");
            }
        }
    }
}
