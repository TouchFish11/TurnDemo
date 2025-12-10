
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

        protected override void Awake()
        {
            base.Awake();

            svTask = uIComponentBinder.GetControl<ScrollRect>(nameof(svTask));
            txtTaskName = uIComponentBinder.GetControl<TextMeshProUGUI>(nameof(txtTaskName));
            txtTaskDescription = uIComponentBinder.GetControl<TextMeshProUGUI>(nameof(txtTaskDescription));
            rewardBox = this.transform.Find(nameof(detailView)).Find(nameof(rewardBox));
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
            }
        }
    }
}
