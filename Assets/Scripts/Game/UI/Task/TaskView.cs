
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

        protected override void Awake()
        {
            base.Awake();

            svTask = uIComponentBinder.GetControl<ScrollRect>(nameof(svTask));
            toggleGroup = svTask.content.GetComponent<ToggleGroup>();
            txtTaskName = uIComponentBinder.GetControl<TextMeshProUGUI>(nameof(txtTaskName));
            txtTaskDescription = uIComponentBinder.GetControl<TextMeshProUGUI>(nameof(txtTaskDescription));
            rewardBox = this.transform.Find(nameof(detailView)).Find(nameof(rewardBox));

            detailView = this.transform.Find(nameof(detailView));
            hasTaskView = detailView.transform.Find(nameof(hasTaskView)).gameObject;
            noTaskView = detailView.transform.Find(nameof(noTaskView)).gameObject;
        }

        public override void UpdateView(string key, object value)
        {
            switch (key)
            {
                case "currentTaskInfo":
                    TaskModel.DetailData detailData = (TaskModel.DetailData)value;

                    txtTaskName.text = detailData.TaskInfo.f_taskName;
                    txtTaskDescription.text = detailData.TaskInfo.f_taskDescription;
                    foreach (ItemGrid itemGrid in detailData.RewardItems)
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
            }
        }

        public ToggleGroup ToggleGroup => toggleGroup;
    }
}
