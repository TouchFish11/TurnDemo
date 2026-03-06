using Core.Collection;
using Core.Log;
using Core.Service;
using HotUpdate.Core;
using HotUpdate.Task.Data;

namespace HotUpdate.Task.Core
{
    public static class TaskUtility
    {
        public static TaskDataCollection GetTaskDataCollection()
        {
            // 转换集合
            if (ServiceLocator.Get<IGameManager>().GameDataManager.TaskDataCollection is Collection<string, TaskData> collection)
            {
                return collection as TaskDataCollection;
            }

            LogManager.LogError($"{nameof(TaskUtility)}.{nameof(GetTaskDataCollection)}：任务数据集合转换失败");
            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string TaskTypeToStr(this int i)
        {
            E_TaskType taskType = (E_TaskType)i;
            return taskType switch
            {
                E_TaskType.MainStory => "主线",
                E_TaskType.SideStroy => "支线",
                _ => ""
            };
        }

        public static E_TaskContentType ToTaskContentType(this int i)
        {
            return (E_TaskContentType)i;
        }
    }
}
