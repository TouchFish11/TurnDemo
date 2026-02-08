using System;
using Core.AssetBundles.Update.Collection;
using Core.Utility;
using Game.Tasks;

namespace GameHotUpdate.Tasks
{
    /// <summary>
    /// 任务数据集合类
    /// 用于管理游戏中所有任务数据的存储、查询等操作，继承自通用键值集合类
    /// </summary>
    [Serializable]
    public class TaskDataCollection : Collection<string, TaskData>, ITaskDataCollection
    {
        /// <summary>
        /// 检查是否包含指定标识的任务（按任务主ID匹配，截取前7位作为主ID）
        /// </summary>
        /// <param name="id">待检查的任务完整标识</param>
        /// <returns>存在返回true，不存在返回false</returns>
        public bool ContainTask(string id)
        {
            // 遍历所有缓存的任务标识
            foreach (var cacheId in keyToValueMap.Keys)
            {
                // 截取任务标识前7位进行匹配（主ID匹配）
                if (TextUtility.Split(cacheId, 7)[0] == TextUtility.Split(id, 7)[0])
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查指定ID的任务是否已完成
        /// </summary>
        /// <param name="taskId">任务唯一标识</param>
        /// <returns>任务完成返回true，未完成/不存在抛出键不存在异常</returns>
        public bool IsFinished(string taskId)
        {
            // 从集合中获取对应任务数据，返回其完成状态
            return keyToValueMap[taskId].isCompleted;
        }

        /// <summary>
        /// 检查是否存在正在追踪的任务
        /// </summary>
        /// <param name="taskData">输出参数，返回第一个正在追踪的任务数据；无则返回null</param>
        /// <returns>存在正在追踪的任务返回true，否则返回false</returns>
        public bool IsTracking(out ITaskData taskData)
        {
            // 遍历所有缓存的任务数据
            foreach (var cacheTaskData in keyToValueMap.Values)
            {
                // 检查任务是否处于追踪状态
                if (!cacheTaskData.isTracking)
                {
                    continue;
                }
                
                taskData = cacheTaskData;
                return true;
            }
            // 无追踪任务时初始化输出参数为null
            taskData = null;
            return false;
        }
    }
}