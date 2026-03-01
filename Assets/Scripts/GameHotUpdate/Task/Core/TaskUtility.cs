using Core.Collection;
using Core.Log;
using Core.Service;
using GameHotUpdate.Main.Manager;
using GameHotUpdate.Task.Data;

namespace GameHotUpdate.Task.Core
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
    }
}
