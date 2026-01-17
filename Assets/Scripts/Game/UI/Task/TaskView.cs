using Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// 任务界面
    /// </summary>
    public class TaskView : UIView
    {
        [Inject] private ScrollRect svTask;
        [Inject] private Button btnAcceptTask;
        [Inject] private ToggleGroup taskContent;
        [Inject] private TextMeshProUGUI txtTaskName;
        [Inject] private TextMeshProUGUI txtTaskDescription;
        [Inject] private TextMeshProUGUI txtAccceptInfo;

        [Inject(1)] private RectTransform detailView;
        [Inject(1)] private RectTransform rewardBox;
        [Inject(1)] private RectTransform hasTaskView;
        [Inject(1)] private RectTransform noTaskView;

        [System.Obsolete]
        public override void UpdateView(string key, object value)
        {
            switch (key)
            {
                case "currentTaskInfo":
                    Clear();
                    (TaskInfo taskinfo, List<ItemGrid> itemGrids) = ((TaskInfo taskinfo, List<ItemGrid> itemGrids))value;
                    txtTaskName.text = taskinfo.f_taskName;
                    txtTaskDescription.text = taskinfo.f_taskDescription;
                    foreach (ItemGrid itemGrid in itemGrids)
                    {
                        itemGrid.transform.SetParent(rewardBox, false);
                    }
                    break;
                case "taskTypeContainer":
                    TaskTypeContainer taskTypeContainer = value as TaskTypeContainer;
                    taskTypeContainer.transform.SetParent(svTask.content, false);
                    break;
                case "hasTasks":
                    bool hasTasks = (bool)value;
                    hasTaskView.gameObject.SetActive(hasTasks);
                    noTaskView.gameObject.SetActive(!hasTasks);
                    break;
                case "isFollowingTask":
                    bool isFollowingTask = (bool)value;
                    txtAccceptInfo.text = isFollowingTask ? "取消追踪" : "开始追踪";
                    break;
            }
        }

        private void Clear()
        {
            int childCount = rewardBox.childCount;
            for (int i = 0; i < childCount; i++)
            {
                PoolManager.Instance.PushObj(rewardBox.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 任务项组
        /// </summary>
        public ToggleGroup TaskItemGroup => taskContent;
    }
}
