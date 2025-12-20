
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
        private ScrollRect svTask;
        private TextMeshProUGUI txtTaskName;
        private TextMeshProUGUI txtTaskDescription;
        private Transform detailView;
        private Transform rewardBox;

        private ToggleGroup toggleGroup;

        private GameObject hasTaskView;
        private GameObject noTaskView;

        private Button btnAcceptTask;
        private TextMeshProUGUI txtAccceptInfo;

        protected override void Awake()
        {
            base.Awake();

            svTask = binder.GetControl<ScrollRect>(nameof(svTask));
            toggleGroup = svTask.content.GetComponent<ToggleGroup>();
            txtTaskName = binder.GetControl<TextMeshProUGUI>(nameof(txtTaskName));
            txtTaskDescription = binder.GetControl<TextMeshProUGUI>(nameof(txtTaskDescription));

            detailView = this.transform.Find(nameof(detailView));
            hasTaskView = detailView.transform.Find(nameof(hasTaskView)).gameObject;
            noTaskView = detailView.transform.Find(nameof(noTaskView)).gameObject;
            rewardBox = this.transform.Find(nameof(detailView)).Find(nameof(hasTaskView)).Find(nameof(rewardBox));

            btnAcceptTask = binder.GetControl<Button>(nameof(btnAcceptTask));
            txtAccceptInfo = binder.GetControl<TextMeshProUGUI>(nameof(txtAccceptInfo));
        }

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
                    hasTaskView.SetActive(hasTasks);
                    noTaskView.SetActive(!hasTasks);
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
        public ToggleGroup TaskItemGroup => toggleGroup;
    }
}
