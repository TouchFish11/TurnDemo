
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MVC
{
    /// <summary>
    /// 任务界面
    /// </summary>
    public class TaskView : UIView
    {
        private ScrollRect svTask;
        private TextMeshProUGUI txtTaskName;
        private TextMeshProUGUI txtTaskDescription;
        private Transform rewardBox;

        protected override void Awake()
        {
            base.Awake();

            svTask = uIComponentBinder.GetControl<ScrollRect>(nameof(svTask));
            txtTaskName = uIComponentBinder.GetControl<TextMeshProUGUI>(nameof(txtTaskName));
            txtTaskDescription = uIComponentBinder.GetControl<TextMeshProUGUI>(nameof(txtTaskDescription));
            rewardBox = this.transform.Find(nameof(rewardBox));
        }

        public override void UpdateView(string key, object value)
        {
            switch (key)
            {

                default:
                    break;
            }
        }
    }
}
