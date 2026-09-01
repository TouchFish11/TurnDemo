using System;
using System.Threading.Tasks;
using Core.Log;
using Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = Core.Log.Logger;

namespace HotUpdate.UI.Activity.Base
{
    /// <summary>
    /// 活动选项UI
    /// </summary>
    public class ActivityUI : UIBehaviourBase
    {
        [InjectUI] private Image imgIcon;
        [InjectUI] private TextMeshProUGUI txtName;
        [InjectUI] private Toggle togActivity;

        public int ActivityId { get; private set; }

        public event Func<int, Task> OnSelect;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="activityInfo"></param>
        /// <param name="toggleGroup"></param>
        public void Init(Sprite icon, ActivityInfo activityInfo, ToggleGroup toggleGroup)
        {
            ActivityId = activityInfo.f_id;
            togActivity.group =  toggleGroup;
            imgIcon.sprite = icon;
            txtName.text = activityInfo.f_name;
        }

        /// <summary>
        /// 选中活动
        /// </summary>
        public async Task SetSelected()
        {
            await TriggerSelectEvent();
            togActivity.isOn = true;
        }
        
        /// <summary>
        /// 选中活动，不触发事件
        /// </summary>
        /// <param name="isOn"></param>
        public void SetSelectedWithoutNotify(bool isOn)
        {
            togActivity.SetIsOnWithoutNotify(isOn);   // 不触发 OnToggleValueChanged
        }

        private Task TriggerSelectEvent()
        {
            return OnSelect != null ? OnSelect?.Invoke(ActivityId) : Task.CompletedTask;
        }

        protected override async void OnToggleValueChanged(string togName, bool isOn)
        {
            try
            {
                if (isOn)
                {
                    await TriggerSelectEvent();
                }
            }
            catch (Exception exception)
            {
                Logger.LogException(ELogTags.Activity, exception);
            }
        }
    }
}