using Core.Collection;
using Core.Log;
using Core.Service;
using HotUpdate.Config.Quest;
using HotUpdate.Core.Manager;
using HotUpdate.Core.Task;

namespace HotUpdate.Task.Core
{
    public static class TaskUtility
    {
        public static IQuestCollection GetTaskDataCollection()
        {
            var taskDataCollection = ServiceLocator.Get<IGameManager>().GameDataManager.GetProvider<ITaskDataProvider>().QuestCollection;
            // 转换集合
            if (taskDataCollection is Collection<int, QuestData> collection)
            {
                return collection as IQuestCollection;
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
