using Core.AssetBundles.Update.Collection;
using Core.Collection;
using Core.Log;
using Core.Service;
using GameHotUpdate.Manager;

namespace GameHotUpdate.Tasks
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
