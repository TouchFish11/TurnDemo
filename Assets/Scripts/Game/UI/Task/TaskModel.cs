using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MVC
{
    /// <summary>
    /// 任务界面数据
    /// </summary>
    public class TaskModel : UIModel
    {
        private readonly List<TaskTypeContainer> taskTypeContainers = new List<TaskTypeContainer>();
        // 
        private readonly List<ItemGrid> rewardItems = new List<ItemGrid>();
        // 当前任务信息
        private TaskInfo currentTaskInfo;

    }
}
